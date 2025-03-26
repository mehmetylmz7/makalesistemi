# -- coding: utf-8 --

import sys
import fitz
import re
import json

# Anonimleþtirilecek kelimeleri tespit eden Regex kurallarý
regex_patterns = {
    "EMAIL": r"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}",
    "KISI": r"\b[A-Z][a-z]+ [A-Z][a-z]+\b",
    "UNIVERSITE": r"\b(?:University|Institute|College|School|Department|Faculty|Lab|Research|Center|Academy)\b"
}

def extract_entities(text):
    entities = []

    for label, pattern in regex_patterns.items():
        matches = re.findall(pattern, text)
        for match in matches:
            entities.append(match)

    return entities

def extract_text_from_pdf(pdf_path):
    doc = fitz.open(pdf_path)
    return "\n".join([page.get_text("text") for page in doc])

if __name__ == "__main__":
    pdf_path = sys.argv[1]
    extracted_text = extract_text_from_pdf(pdf_path)
    detected_entities = extract_entities(extracted_text)

    print(json.dumps(detected_entities))
