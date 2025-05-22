import requests
from flask import Blueprint, jsonify, request, send_file
from Libs.topic import getTopicsFromPdf
from Libs.anonym import anonym_pdf

nlp_controller = Blueprint("nlp_controller",__name__)

NETCORE_PATH ="http://localhost:5253/"

@nlp_controller.route('getTopics', methods=["GET"])
def get_topics():
    folder = request.args.get("path")
    filename= "./Files/"+ request.args.get("filename")
    if not folder or not filename:
        return jsonify("Cannot read directory path"),404
    
    response = requests.get(f"{NETCORE_PATH}{folder}original.pdf")
    if response.status_code!=200:
        return jsonify("Cannot read file"),404
    
    text = getTopicsFromPdf(filename,response.content)

    return jsonify(text),201


@nlp_controller.route('anonym', methods=["PUT"])
def anonym():
    data = request.get_json()

    if not data:
        print("Missing data")
        return jsonify("missing data"),404

    pdf_Path= data["PdfPath"]
    pdf_name=data["PdfName"]
    hideName=data["HideName"]
    hideCompany=data["HideCompany"]
    hideMailPhone=data["HideMailPhone"]

    if not pdf_Path or not pdf_name or not isinstance(hideName,bool) or not isinstance(hideCompany,bool) or not isinstance(hideMailPhone,bool):
        print("Cannot read properties")
        return jsonify("Cannot read properties"),404
    
    response = requests.get(f"{NETCORE_PATH}{pdf_Path}")

    if response.status_code!=200:
        print("cannot read file"+f"{pdf_Path}"+f"{response.status_code}")
        return jsonify("Cannot read file"),404

    input_pdf_path="./Files/_temp"+pdf_name
    output_pdf_path="./Files/"+pdf_name

    output_pdf = anonym_pdf(response.content,input_pdf_path,output_pdf_path,hideName,hideCompany,hideMailPhone)

    return send_file(output_pdf,as_attachment=True)