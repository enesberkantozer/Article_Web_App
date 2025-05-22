import fitz
import os

def addReviewNewPageFromArray(blobData,tempPath,commentArray):
    with open("./Files/"+"temp_"+tempPath, "wb") as f:
        f.write(blobData)

    pdf_document = fitz.open("./Files/"+"temp_"+tempPath)

    for comment in commentArray:
        new_page = pdf_document.new_page(width=595, height=842)

        ratingvalue=comment["Ratingvalue"]
        time=comment["Time"]
    
        new_page.insert_text((50, 50), f"Rating: {ratingvalue}", fontsize=12, fontname="helv", color=(0, 0, 0))
        new_page.insert_text((50, 80), f"Date: {time}", fontsize=12, fontname="helv", color=(0, 0, 0))
        new_page.insert_text((50, 110), comment["Title"], fontsize=20, fontname="helv", color=(0, 0, 0))
        new_page.insert_text((50, 150), comment["Comment"], fontsize=12, fontname="helv", color=(0, 0, 0))

    pdf_document.save("./Files/"+tempPath)
    pdf_document.close()
    os.remove("./Files/"+"temp_"+tempPath)

    return "./Files/"+tempPath