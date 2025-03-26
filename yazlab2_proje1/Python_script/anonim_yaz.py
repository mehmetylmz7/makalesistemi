import sys
import fitz  # PyMuPDF
import json
import os

def anonymize_pdf(input_pdf, output_pdf, replacements):
    doc = fitz.open(input_pdf)
    for page in doc:
        for item in replacements:
            text_instances = page.search_for(item)
            for inst in text_instances:
                page.add_redact_annot(inst, fill=(1, 1, 1)) 

        page.apply_redactions()

    try:
        doc.save(output_pdf)
        log = f"[OK] PDF kaydedildi: {output_pdf}\n"
    except Exception as e:
        log = f"[ERROR] Kaydetme hatasi: {str(e)}\n"

    with open("anonim_log.txt", "w", encoding="utf-8") as f:
        f.write(f"input_pdf: {input_pdf}\n")
        f.write(f"output_pdf: {output_pdf}\n")
        f.write(f"Dosya var mi?: {os.path.exists(output_pdf)}\n")
        f.write(log)

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
