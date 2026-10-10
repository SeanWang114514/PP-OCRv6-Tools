using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

public class ResPatch {
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern IntPtr LoadLibraryEx(string f, IntPtr h, uint flags);
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool FreeLibrary(IntPtr h);
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern IntPtr FindResource(IntPtr h, IntPtr name, IntPtr type);
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr LoadResource(IntPtr h, IntPtr res);
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern uint SizeofResource(IntPtr h, IntPtr res);
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr LockResource(IntPtr p);
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern IntPtr BeginUpdateResource(string file, bool del);
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool UpdateResource(IntPtr h, IntPtr type, IntPtr name, ushort lang, byte[] data, uint cb);
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool EndUpdateResource(IntPtr h, bool discard);

    public delegate bool EnumLangsProc(IntPtr h, IntPtr type, IntPtr name, ushort lang, IntPtr lParam);
    public delegate bool EnumNamesProc(IntPtr h, IntPtr type, IntPtr name, IntPtr lParam);
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool EnumResourceLanguages(IntPtr h, IntPtr type, IntPtr name, EnumLangsProc cb, IntPtr lParam);
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool EnumResourceNames(IntPtr h, IntPtr type, EnumNamesProc cb, IntPtr lParam);

    public class Frame { public int W, H; public ushort Planes, Bpp; public byte[] Data; }

    public static List<Frame> ParseIco(string path) {
        byte[] b = File.ReadAllBytes(path);
        int count = BitConverter.ToUInt16(b, 4);
        var frames = new List<Frame>();
        for (int i = 0; i < count; i++) {
            int off = 6 + i * 16;
            int w = b[off]; if (w == 0) w = 256;
            int h = b[off + 1]; if (h == 0) h = 256;
            ushort planes = BitConverter.ToUInt16(b, off + 4);
            ushort bpp = BitConverter.ToUInt16(b, off + 6);
            uint len = BitConverter.ToUInt32(b, off + 8);
            int dataOff = (int)BitConverter.ToUInt32(b, off + 12);
            byte[] data = new byte[len];
            Array.Copy(b, dataOff, data, 0, (int)len);
            bool isPng = data.Length > 8 && data[0] == 0x89 && data[1] == 0x50;
            frames.Add(new Frame { W = w, H = h, Planes = isPng ? (ushort)0 : planes, Bpp = isPng ? (ushort)0 : bpp, Data = data });
        }
        return frames;
    }

    public static byte[] BuildGroup(List<Frame> frames, int[] ids) {
        var ms = new MemoryStream();
        var w = new BinaryWriter(ms);
        w.Write((ushort)0); w.Write((ushort)1); w.Write((ushort)frames.Count);
        for (int i = 0; i < frames.Count; i++) {
            var f = frames[i];
            w.Write((byte)(f.W >= 256 ? 0 : f.W));
            w.Write((byte)(f.H >= 256 ? 0 : f.H));
            w.Write((byte)0); w.Write((byte)0);
            w.Write(f.Planes); w.Write(f.Bpp);
            w.Write((uint)f.Data.Length);
            w.Write((ushort)ids[i]);
        }
        return ms.ToArray();
    }

    public static bool Patch(string exePath, string icoPath, out string error) {
        error = null;
        var frames = ParseIco(icoPath);
        IntPtr h = LoadLibraryEx(exePath, IntPtr.Zero, 0x00000002);
        if (h == IntPtr.Zero) { error = "LoadLibraryEx failed: " + Marshal.GetLastWin32Error(); return false; }
        var langs = new List<ushort>();
        EnumResourceLanguages(h, (IntPtr)14, (IntPtr)1,
            (hm, t, n, lang, lp) => { langs.Add(lang); return true; }, IntPtr.Zero);
        var iconIds = new List<int>();
        EnumResourceNames(h, (IntPtr)3,
            (hm, t, name, lp) => {
                long v = name.ToInt64();
                if (v > 0 && v <= 0xFFFF) iconIds.Add((int)v);
                return true;
            }, IntPtr.Zero);
        FreeLibrary(h);
        if (langs.Count == 0) langs.Add(0);
        Console.WriteLine("langs: " + string.Join(",", langs));
        Console.WriteLine("old RT_ICON ids: " + string.Join(",", iconIds));

        int[] ids = new int[frames.Count];
        for (int i = 0; i < frames.Count; i++) ids[i] = 11 + i;
        byte[] group = BuildGroup(frames, ids);

        IntPtr upd = BeginUpdateResource(exePath, false);
        if (upd == IntPtr.Zero) { error = "BeginUpdateResource failed: " + Marshal.GetLastWin32Error(); return false; }
        bool ok = true;
        foreach (ushort lang in langs) {
            // 删除旧的 RT_ICON 资源
            foreach (int id in iconIds)
                UpdateResource(upd, (IntPtr)3, (IntPtr)id, lang, null, 0);
            // 写入新的 RT_ICON 帧
            for (int i = 0; i < frames.Count; i++) {
                if (!UpdateResource(upd, (IntPtr)3, (IntPtr)ids[i], lang, frames[i].Data, (uint)frames[i].Data.Length)) {
                    error = "UpdateResource icon id " + ids[i] + " failed: " + Marshal.GetLastWin32Error();
                    ok = false; break;
                }
            }
            if (!ok) break;
            if (!UpdateResource(upd, (IntPtr)14, (IntPtr)1, lang, group, (uint)group.Length)) {
                error = "UpdateResource group failed: " + Marshal.GetLastWin32Error();
                ok = false; break;
            }
        }
        if (!ok) { EndUpdateResource(upd, true); return false; }
        if (!EndUpdateResource(upd, false)) { error = "EndUpdateResource failed: " + Marshal.GetLastWin32Error(); return false; }
        return true;
    }

    // 验证：解析补丁后的 group icon，返回每帧尺寸+数据哈希
    public static string Verify(string exePath) {
        var sb = new System.Text.StringBuilder();
        IntPtr h = LoadLibraryEx(exePath, IntPtr.Zero, 0x00000002);
        if (h == IntPtr.Zero) return "LoadLibraryEx failed";
        try {
            IntPtr grp = FindResource(h, (IntPtr)1, (IntPtr)14);
            if (grp == IntPtr.Zero) return "no group icon";
            IntPtr gh = LoadResource(h, grp);
            uint gsz = SizeofResource(h, grp);
            IntPtr gp = LockResource(gh);
            byte[] g = new byte[gsz];
            Marshal.Copy(gp, g, 0, (int)gsz);
            int count = BitConverter.ToUInt16(g, 4);
            sb.Append("group entries: " + count + "\n");
            for (int i = 0; i < count; i++) {
                int off = 6 + i * 14;
                int w = g[off]; if (w == 0) w = 256;
                int hh = g[off + 1]; if (hh == 0) hh = 256;
                uint len = BitConverter.ToUInt32(g, off + 8);
                int rid = BitConverter.ToUInt16(g, off + 12);
                IntPtr ic = FindResource(h, (IntPtr)rid, (IntPtr)3);
                if (ic == IntPtr.Zero) { sb.Append("  " + w + "x" + hh + " MISSING RT_ICON " + rid + "\n"); continue; }
                IntPtr ih = LoadResource(h, ic);
                uint ilen = SizeofResource(h, ic);
                IntPtr ip = LockResource(ih);
                byte[] d = new byte[ilen];
                Marshal.Copy(ip, d, 0, (int)ilen);
                using (var sha = System.Security.Cryptography.SHA256.Create()) {
                    byte[] hh2 = sha.ComputeHash(d);
                    sb.Append("  " + w + "x" + hh + " id=" + rid + " len=" + ilen + " sha256=" + BitConverter.ToString(hh2).Replace("-", "").Substring(0, 16) + "\n");
                }
            }
            return sb.ToString();
        } finally { FreeLibrary(h); }
    }
}
