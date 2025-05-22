import os
import fitz
import spacy
from collections import Counter

nlp = spacy.load("en_core_web_sm")

def getTopicsFromPdf(tempFileName,blobData):
    with open(tempFileName,"wb") as f:
        f.write(blobData)
    
    text=extract_text(tempFileName)
    os.remove(tempFileName)

    indexAbstract= text.find("abstract")
    indexReferences = text.rfind("references")
    text = text[indexAbstract + len("abstract"):indexReferences].strip()
    
    words= extract_keywords(text)
    return words

def extract_text(file):
    doc= fitz.open(file)
    text=""
    for page_num in range(doc.page_count):
        page = doc.load_page(page_num)
        text+= page.get_text()
    return text.replace("\n"," ").replace("\t"," ").strip().lower()
    
def extract_keywords(text, num_keywords=10):
    doc = nlp(text)
    
    keywords=[]

    for chunk in doc.noun_chunks:
        if len(chunk.text.split()) > 1:
            keywords.append(chunk.text.lower())
    
    isAddedBefore=False

    for token in doc:
        if token.is_alpha and not token.is_stop:
            for word in keywords:
                if word.find(token.lemma_.lower())>=0:
                    isAddedBefore=True
                    break
            if not isAddedBefore:
                keywords.append(token.lemma_.lower())
            else:
                isAddedBefore:False
    
    word_counts = Counter(keywords)
    
    return word_counts.most_common(num_keywords)