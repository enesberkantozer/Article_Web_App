from flask import Flask
from Controllers.nlp_controller import nlp_controller
from Controllers.review_controller import review_controller

app=Flask(__name__)

app.register_blueprint(nlp_controller, url_prefix="/api/nlp")
app.register_blueprint(review_controller, url_prefix="/api/review")

if(__name__=="__main__"):
    app.run(debug=True)