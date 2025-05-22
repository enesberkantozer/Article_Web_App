import spacy
import fitz
import re

nlp = spacy.load("en_core_web_trf")

email_regex = r"^(mailto:)?[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"
phone_regex = r"\+?[1-9]\d{0,2}[\s-]?\(?\d{1,4}\)?[\s-]?\d{1,4}[\s-]?\d{1,4}[\s-]?\d{1,4}"

def anonym_pdf(blobData,input_pdf, output_pdf, hideName, hideCompany, hideMailPhone):
    with open(input_pdf, "wb") as f:
        f.write(blobData)

    doc = fitz.open(input_pdf)

    for page_num in range(len(doc)):
        page = doc.load_page(page_num)

        text = page.get_text("text")

        doc_spacy = nlp(text)

        for ent in doc_spacy.ents:
            if hideName:
                if ent.label_ == "PERSON":
                    name = ent.text
                    print(f"Bulunan isim: {name}")

                    text_instances = page.search_for(name)

                    for inst in text_instances:
                        x0, y0, x1, y1 = inst 

                        page.add_redact_annot(inst)
                        page.apply_redactions()

                        page.insert_text((x0, y0+5), "[Anonim]", fontsize=8, color=(0, 0, 0))

            if hideCompany:
                if ent.label_ == "ORG":
                    name = ent.text
                    print(f"Bulunan firma ismi: {name}")

                    text_instances = page.search_for(name)

                    for inst in text_instances:
                        x0, y0, x1, y1 = inst 

                        page.add_redact_annot(inst)
                        page.apply_redactions()

                        page.insert_text((x0, y0+5), "[Anonim]", fontsize=8, color=(0, 0, 0))

        if hideMailPhone:
            for match in re.finditer(email_regex, text):
                email = match.group()
                print(f"Bulunan e-posta: {email}")

                text_instances = page.search_for(email)
                for inst in text_instances:
                    x0, y0, x1, y1 = inst

                    page.add_redact_annot(inst)
                    page.apply_redactions()

                    page.insert_text((x0, y0+5), "[Anonim]", fontsize=8, color=(0, 0, 0))

            for match in re.finditer(phone_regex, text):
                phone = match.group()
                print(f"Bulunan telefon numarası: {phone}")

                text_instances = page.search_for(phone)
                for inst in text_instances:
                    x0, y0, x1, y1 = inst

                    page.add_redact_annot(inst)
                    page.apply_redactions()

                    page.insert_text((x0, y0+5), "[Anonim]", fontsize=8, color=(0, 0, 0))

    doc.save(output_pdf)

    return output_pdf