# -*- coding: utf-8 -*-

import sys
import fitz  # PyMuPDF
import json
import os
import random
import string
from Crypto.Cipher import AES
from Crypto.Util.Padding import pad
import base64

# Sabit anahtar ve IV
KEY = b'MakaleAnonimSistemi2024Key123456'  # 32 byte
IV = b'AnonimSistemiIV'  # 16 byte

def encrypt_text(text):
    try:
        # AES þifreleme nesnesi oluþtur
        cipher = AES.new(KEY, AES.MODE_CBC, IV)
        
        # Metni bytes'a çevir ve padding ekle
        text_bytes = text.encode('utf-8')
        padded_text = pad(text_bytes, AES.block_size)
        
        # Metni þifrele
        encrypted_text = cipher.encrypt(padded_text)
        
        # Base64 ile kodla
        return base64.b64encode(encrypted_text).decode('utf-8')
    except Exception as e:
        print(f"Þifreleme hatasý: {str(e)}")
        return ""

def generate_unique_code():
    # 3 harf ve 3 rakamdan oluþan benzersiz kod
    letters = ''.join(random.choices(string.ascii_uppercase, k=3))
    numbers = ''.join(random.choices(string.digits, k=3))
    return f"{letters}{numbers}"

def anonymize_pdf(input_pdf, output_pdf, replacements):
    doc = fitz.open(input_pdf)
    replacements_map = {}
    
    for page in doc:
        text_instances = []
        for item in replacements:
            instances = page.search_for(item)
            for inst in instances:
                text_instances.append((inst, item))
        
        for inst, item in text_instances:
            unique_code = generate_unique_code()
            encrypted_text = encrypt_text(item)
            
            # Her kod için detaylý bilgi sakla
            replacements_map[unique_code] = {
                "original": item,
                "encrypted": encrypted_text
            }
            
            # Redaksiyon için annotasyon ekle
            redact_annot = page.add_redact_annot(inst, fill=(1, 1, 1))
            
            # Redaksiyonu uygula (bu orijinal metni siler)
            page.apply_redactions()
            
            # Yeni metni eklemeden önce doðru pozisyonu hesapla
            # Orijinal bbox'ýn sol alt köþesine metni ekliyoruz
            text_pos = (inst.x0 + 2, inst.y1 - 2)  # Küçük bir padding ekledik
            
            # Yeni kodu ekle (font boyutunu ve rengini ayarla)
            page.insert_text(
                text_pos,
                unique_code,
                fontsize=9,  # Daha okunabilir boyut
                color=(0, 0, 0),  # Siyah renk
                fontname="helv",  # Standart font
            )

    # Deðiþiklikleri kaydet
    try:
        doc.save(output_pdf)
        json_dir = os.path.join(os.path.dirname(output_pdf), "..", "json")
        if not os.path.exists(json_dir):
            os.makedirs(json_dir)
            
        # Input PDF'in adýný al ve JSON dosya adýný oluþtur
        input_pdf_name = os.path.splitext(os.path.basename(input_pdf))[0]
        json_filename = f"{input_pdf_name}_replacements_map.json"
        json_path = os.path.join(json_dir, json_filename)
        
        with open(json_path, "w", encoding="utf-8") as f:
            json.dump(replacements_map, f, ensure_ascii=False, indent=4)
        
        print(f"[SUCCESS] Anonymized PDF saved to: {output_pdf}")
        print(f"[SUCCESS] Mappings saved to: {json_path}")
    except Exception as e:
        print(f"[ERROR] Saving failed: {str(e)}")
        raise
    finally:
        doc.close()

if __name__ == "__main__":
    try:
        input_pdf = sys.argv[1]
        output_pdf = sys.argv[2]
        json_path = sys.argv[3]

        with open(json_path, "r", encoding="utf-8") as f:
            replacements = json.load(f)

        anonymize_pdf(input_pdf, output_pdf, replacements)

    except Exception as e:
        with open("anonim_log.txt", "w", encoding="utf-8") as f:
            f.write(f"[EXCEPTION] {str(e)}\n")
        raise
