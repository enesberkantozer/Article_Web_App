import os
import requests
from flask import Blueprint, jsonify, request, send_file
from Libs.review import addReviewNewPageFromArray

review_controller=Blueprint("review_controller",__name__)

NETCORE_PATH ="http://localhost:5253/"

@review_controller.route("add", methods=["PUT"])
def addReviewFromArray():
    data =request.get_json()

    print(data)

    if not data:
        return jsonify("Missing data"),404

    commentArray=data["Ratinglist"]
    filepath=data["Filepath"]
    tempfilename=data["Tempfilename"]

    if not commentArray or not filepath or not tempfilename:
        return jsonify("Cannot read properties"),404
    
    response=requests.get(f"{NETCORE_PATH}{filepath}")

    if response.status_code!=200:
        print("cannot read file"+filepath+f"{response.status_code}")
        return jsonify("Cannot read file"),404
    
    tempfilename=addReviewNewPageFromArray(response.content,tempfilename,commentArray)

    return send_file(tempfilename,as_attachment=True)