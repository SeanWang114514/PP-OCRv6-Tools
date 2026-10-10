# -*- coding: utf-8 -*-
"""Build a synthetic page that isolates the row-orientation bug.

Row 1 is upright, row 2 is the same sentence rotated 180 degrees, row 3 is small text. With the
old global-vote angle classifier one rotated row flipped every row, so the upright sentence came
out reversed; with the per-line confidence gate each row keeps its own orientation.
"""
from pathlib import Path
from PIL import Image, ImageDraw, ImageFont

OUT = Path(__file__).resolve().parent / "ocr_probe"
OUT.mkdir(exist_ok=True)

W, H = 1400, 620
img = Image.new("RGB", (W, H), "white")
dr = ImageDraw.Draw(img)


def font(px):
    for p in (r"C:\Windows\Fonts\msyh.ttc", r"C:\Windows\Fonts\simhei.ttf"):
        if Path(p).exists():
            try:
                return ImageFont.truetype(p, px)
            except Exception:
                continue
    return ImageFont.load_default()


up = "苹果公司发布新款手机售价5999元"
flip = "第二行文字故意上下颠倒"
small = "这是一段较小的文字用于测试检测分辨率是否足够"

dr.text((60, 40), up, fill="black", font=font(56))
line2 = Image.new("RGB", (900, 110), "white")
ImageDraw.Draw(line2).text((10, 10), flip, fill="black", font=font(56))
img.paste(line2.rotate(180), (60, 200))
dr.text((60, 380), small, fill="black", font=font(26))
dr.rectangle([40, 20, W - 40, H - 40], outline=(200, 200, 200), width=2)

out = OUT / "orient.png"
img.save(out)
print(out)
