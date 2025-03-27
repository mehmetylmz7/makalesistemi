# -*- coding: utf-8 -*-

import sys
import fitz  # PyMuPDF
import json
import os
import random
import string
from Crypto.Cipher import AES
from Crypto.Util.Padding import pad, unpad
from Crypto.Random import get_random_bytes
import base64
import hashlib

def generate_unique_code():
    #"""3 harf ve 3 rakamdan oluþan benzersiz kod üretir"""
    letters = ''.join(random.choices(string.ascii_uppercase, k=3))
    numbers = ''.join(random.choices(string.digits, k=3))
    return f"{letters}{numbers}"

def get_encryption_key():
    #"""Güvenli bir þifreleme anahtarý oluþturur veya var olaný kullanýr"""
    key_path = "encryption_key.bin"
    if os.path.exists(key_path):
        with open(key_path, "rb") as f:
            return f.read()
    else:
        key = get_random_bytes(32)  # AES-256 için 32 byte anahtar
        with open(key_path, "wb") as f:
            f.write(key)
        return key

def encrypt_data(data, key):
    #"""Veriyi AES-256 CBC modunda þifreler"""
    iv = get_random_bytes(16)
    cipher = AES.new(key, AES.MODE_CBC, iv)
    ct_bytes = cipher.encrypt(pad(data.encode('utf-8'), AES.block_size))  # Burada parantez kapatýldý
    return base64.b64encode(iv + ct_bytes).decode('utf-8')

def anonymize_pdf(input_pdf, output_pdf, replacements):
  #  """PDF'deki hassas verileri anonimleþtirir ve þifreli olarak kaydeder"""
    doc = fitz.open(input_pdf)
    replacements_map = {}
    encryption_key = get_encryption_key()
    
    for page in doc:
        text_instances = []
        for item in replacements:
            instances = page.search_for(item)
            for inst in instances:
                text_instances.append((inst, item))
        
        for inst, item in text_instances:
            unique_code = generate_unique_code()
            # Orijinal veriyi þifrele
            encrypted_data = encrypt_data(item, encryption_key)
            replacements_map[unique_code] = encrypted_data
            
            # Redaksiyon iþlemi
            redact_annot = page.add_redact_annot(inst, fill=(1, 1, 1))
            page.apply_redactions()
            
            # Yeni kodu ekle
            text_pos = (inst.x0 + 2, inst.y1 - 2)
            page.insert_text(
                text_pos,
                unique_code,
                fontsize=5,
                color=(0, 0, 0),
                fontname="helv",
            )

    # Deðiþiklikleri kaydet
    try:
        doc.save(output_pdf)
        json_dir = os.path.join(os.path.dirname(output_pdf), "..", "json")
        if not os.path.exists(json_dir):
            os.makedirs(json_dir)
            
        input_pdf_name = os.path.splitext(os.path.basename(input_pdf))[0]
        json_filename = f"{input_pdf_name}_replacements_map.json"
        json_path = os.path.join(json_dir, json_filename)
        
        with open(json_path, "w", encoding="utf-8") as f:
            json.dump(replacements_map, f, ensure_ascii=False, indent=4)
        
        print(f"[SUCCESS] Anonymized PDF saved to: {output_pdf}")
        print(f"[SUCCESS] Encrypted mappings saved to: {json_path}")
        print(f"[INFO] Encryption key saved to: encryption_key.bin - KEEP THIS SAFE!")
    except Exception as e:
        print(f"[ERROR] Saving failed: {str(e)}")
        raise
    finally:
        doc.close()

if __name__ == "__main__":
    try:
        if len(sys.argv) != 4:
            print("Usage: python script.py <input_pdf> <output_pdf> <replacements_json>")
            sys.exit(1)
            
        input_pdf = sys.argv[1]
        output_pdf = sys.argv[2]
        json_path = sys.argv[3]

        with open(json_path, "r", encoding="utf-8") as f:
            replacements = json.load(f)

        anonymize_pdf(input_pdf, output_pdf, replacements)

    except Exception as e:
        error_msg = f"[EXCEPTION] {str(e)}"
        print(error_msg)
        with open("anonim_log.txt", "a", encoding="utf-8") as f:
            f.write(f"{error_msg}\n")
        sys.exit(1)