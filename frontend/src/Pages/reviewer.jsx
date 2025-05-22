import React, { useState, useEffect } from "react";
import { Form, Button, Spinner, ToastContainer, Toast, Table, Row, Col, Modal } from "react-bootstrap";
import axios from "axios";
import "../Components/rateStar.css";

const Reviewer = () => {
    const [email, setEmail] = useState(null);
    const [status, setStatus] = useState("login");
    const [progressTimer, setProgressTimer] = useState(180);
    const [percentage, setPercentage] = useState("100%");
    const [isTimerActive, setIsTimerActive] = useState(false);
    const [verificationCode, setVerificationCode] = useState(0);
    const [toast, setToast] = useState({ title: "", message: "", show: false });
    const [me, setMe] = useState([]);
    const [myArticles, setMyArticles] = useState([]);
    const [currentArticle, setCurrentArticle] = useState([]);
    const [pdfUrl, setPdfUrl] = useState(null);
    const [starValue, setStarValue] = useState(0);
    const [commentTitle, setCommentTitle] = useState("");
    const [comment, setComment] = useState("");
    const [showModal, setShowModal] = useState(false);
    useEffect(() => {
        let interval;
        if (isTimerActive && progressTimer > 0) {
            interval = setInterval(() => {
                setProgressTimer((prev) => {
                    const newProgressTimer = prev - 1;
                    setPercentage(Math.floor((newProgressTimer * 100) / 180) + "%");
                    return newProgressTimer;
                });
            }, 1000);
        } else if (progressTimer <= 0) {
            setIsTimerActive(false);
            setProgressTimer(180);
            setStatus("login");
        }
        return () => clearInterval(interval);
    }, [isTimerActive, progressTimer]);

    const getMyArticles = async (mail) => {
        axios.get("http://localhost:5253/api/Reviewers/Get/" + encodeURIComponent(mail))
            .then(response => {
                setMe(response.data);
                setMyArticles(response.data.articles);
            }).catch(error => {
                console.error(error);
                setToast({ title: "Hata", message: "Makaleleriniz getirilemedi", show: true });
            })
    }

    const prepareCommentPanel = async (id) => {
        const a = myArticles.filter(a => a.id === id)[0];
        setCurrentArticle(a);

        axios.get("http://localhost:5253/" + a.filePath + "/anonym.pdf", {
            responseType: 'blob'
        }).then(response => {
            setPdfUrl(URL.createObjectURL(response.data));
            setStatus("comment");
        }).catch(error => {
            console.error(error);
            setToast({ title: "Hata", message: "Veri çekilemedi", show: true });
        });
    }

    const handleMailChange = (e) => {
        setEmail(e.target.value);
    }

    const handleNumberChange = (e) => {
        setVerificationCode(e.target.value);
    }

    const handleCommentTitle = (e) => {
        setCommentTitle(e.target.value);
    }

    const handleCommentContext = (e) => {
        setComment(e.target.value);
    }

    const handleMailSubmit = async (e) => {
        e.preventDefault();

        setStatus("loading");

        axios.get("http://localhost:5253/api/Reviewers/isReviewer/" + encodeURIComponent(email))
            .then(response => {
                if (response.data === false) {
                    setStatus("login");
                    setToast({ title: "Uyarı", message: "Sistemizde böyle bir hakem kayıtlı değil", show: true });
                }
                else {
                    axios.post('http://localhost:5253/api/Mails/sendVerification/' + encodeURIComponent(email))
                        .then(() => {
                            setStatus("verification");
                            setIsTimerActive(true);
                        }).catch(error => {
                            console.error("Hata:", error);
                            setStatus("login");
                            setToast({ title: "Hata", message: "Doğrulama kodu gönderilemedi", show: true });
                        });
                }
            }).catch(error => {
                console.error(error);
                setStatus("login");
                setToast({ title: "Hata", message: "Tekrar deneyin", show: true });
            });
    }

    const handleCheckVerification = async (e) => {
        e.preventDefault();

        if (!(await checkCode(email, verificationCode))) {
            setToast({ title: "Uyarı", message: "Doğrulama kodu yanlış", show: true });
            return;
        }

        setIsTimerActive(false);
        setProgressTimer(180);
        setPercentage("100%");
        setStatus("loading");

        await deleteAllVerificationCode(email);

        await getMyArticles(email);
        setStatus("articles");
    };

    const checkCode = async (mail, code) => {
        return await axios.get('http://localhost:5253/api/Mails/check/' + encodeURIComponent(mail))
            .then(response => {
                return (parseInt(code) === parseInt(response.data));
            })
            .catch(error => {
                console.error(error);
                return false;
            });
    }

    const deleteAllVerificationCode = async (mail) => {
        await axios.delete('http://localhost:5253/api/Mails/deleteVerification/' + encodeURIComponent(email))
            .then(() => {
                console.log("Successfully deleted verifications");
            }).catch(() => {
                console.error("Verifications delete failed");
            });
    }

    const sendReview = async (e) => {
        e.preventDefault();

        axios.post("http://localhost:5253/api/Reviewers/AddReview", {
            ratingValue: starValue,
            title: commentTitle,
            comment: comment,
            articleId: currentArticle.id,
            reviewerId: me.id
        }).then(() => {
            setShowModal(false);
            getMyArticles(me.mail);
            setStatus("articles");
            URL.revokeObjectURL(pdfUrl);
            setPdfUrl(null);
            setCurrentArticle([]);
            setStarValue(0);
            setCommentTitle("");
            setComment("");
            setToast({ title: "Başarılı", message: "Yorum eklendi", show: true });
        }).catch(error => {
            console.error(error);
            setToast({ title: "Hata", message: "Yorumu tekrar göndermeyi deneyin", show: true });
        });
    }

    const handleBack = async (e) => {
        e.preventDefault();

        URL.revokeObjectURL(pdfUrl);
        setPdfUrl(null);
        setCurrentArticle([]);
        setStarValue(0);
        setCommentTitle("");
        setComment("");
        setStatus("articles");
    }

    const containerStyle = {
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        height: "100vh",
        background: "linear-gradient(90deg, #0f0c29 0%, #302b63 50%, #24243e 100%)"
    };

    const divStyleForm = {
        width: "40%",
        height: "50%",
        padding: "20px",
        border: "1px solid #ccc",
        borderRadius: "25px",
        background: "#fff"
    };

    const divStylePanel = {
        marginTop: '8vh',
        width: "90%",
        height: "88%",
        padding: "20px",
        border: "1px solid #ccc",
        borderRadius: "25px",
        background: "#fff",
        overflow: "auto"
    };

    return (
        <div style={containerStyle}>
            {status !== "comment" && (
                <div style={divStyleForm}>
                    {status === "login" && (
                        <>
                            <h2 style={{ textAlign: 'center' }}>Hakem Sayfası</h2>
                            <Form onSubmit={handleMailSubmit} style={{ height: '100%', margin: '12vh 3vw' }}>
                                <Form.Floating className="mb-3">
                                    <Form.Control
                                        id="floatingInputCustom"
                                        type="email"
                                        placeholder="name@example.com"
                                        onChange={handleMailChange}
                                    />
                                    <label htmlFor="floatingInputCustom">Email adresi</label>
                                </Form.Floating>
                                <div className="d-grid gap-2">
                                    <Button type='submit'>Giriş Yap</Button>
                                </div>
                            </Form>
                        </>
                    )}

                    {status === "loading" && (
                        <div style={{ display: 'flex', flexDirection: 'column', width: '100%', height: '100%', justifyContent: 'center', alignItems: 'center' }}>
                            <Spinner animation='grow' variant='primary' style={{ width: '5rem', height: '5rem' }} />
                            <h2 style={{ textAlign: 'center' }}>Yükleniyor...</h2>
                        </div>
                    )}

                    {status === "verification" && (
                        <div style={{ height: "100%" }}>
                            <h2 style={{ width: '100%', textAlign: 'center' }}>Doğrulama Kodu</h2><br />
                            <form onSubmit={handleCheckVerification} style={{ margin: '4% auto' }}>
                                <p style={{ textAlign: 'center' }}>6 haneli doğrulama kodunuzu girin</p>
                                <input onChange={handleNumberChange} className="form-control" style={{ textAlign: 'center', fontSize: '2em', width: '50%', margin: 'auto' }} type="number" required />
                                <div className="progress" style={{ margin: '5% 10%', height: "4vh" }}>
                                    <div className="progress-bar bg-success" role="progressbar" style={{ width: percentage }}
                                        aria-valuenow={progressTimer} aria-valuemin="0" aria-valuemax="180">
                                        {progressTimer} sn
                                    </div>
                                </div>
                                <button type='submit' class="btn btn-primary" style={{ textAlign: 'center', width: '100%', margin: '5% 0%' }}>Doğrula</button>
                            </form>
                        </div>
                    )}

                    {status === "articles" && (
                        <Table striped responsive hover style={{ maxWidth: '100%', maxHeight: '100%' }}>
                            <thead>
                                <tr>
                                    <th>#</th>
                                    <th>Id</th>
                                    <th>Makale Başlığı</th>
                                    <th style={{ textAlign: 'center' }}>İşlemler</th>
                                </tr>
                            </thead>
                            <tbody>
                                {myArticles.map((row, index) => (
                                    <tr key={index}>
                                        <td>{index + 1}</td>
                                        <td>{row.id}</td>
                                        <td>{row.title}</td>
                                        <td style={{ display: 'flex', justifyContent: 'center' }}>
                                            <Button onClick={() => prepareCommentPanel(row.id)}>İşlem Yap</Button>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </Table>
                    )}
                </div>
            )}

            {status === "comment" && (
                <div style={divStylePanel}>
                    <Row style={{ height: '100%' }}>
                        <Col style={{ height: "100%", borderRight: "1px solid blue" }}>
                            <iframe src={pdfUrl} style={{ height: '100%', width: '100%', border: '1px solid black' }} title="Makale"></iframe>
                        </Col>
                        <Col style={{ height: '100%' }}>
                            <Row><Col>ID: {currentArticle.id}</Col><Col><h3 style={{ textAlign: "center" }}>{currentArticle.title}</h3></Col><Col></Col></Row>
                            <div style={{ display: 'flex', justifyContent: 'center' }}>
                                <fieldset class="rating">
                                    <input onClick={() => setStarValue(5)} type="radio" id="star5" name="rating" value="5" /><label class="full" for="star5" title="Awesome - 5 stars"></label>
                                    <input onClick={() => setStarValue(4.5)} type="radio" id="star4half" name="rating" value="4.5" /><label class="half" for="star4half" title="Pretty good - 4.5 stars"></label>
                                    <input onClick={() => setStarValue(4)} type="radio" id="star4" name="rating" value="4" /><label class="full" for="star4" title="Pretty good - 4 stars"></label>
                                    <input onClick={() => setStarValue(3.5)} type="radio" id="star3half" name="rating" value="3.5" /><label class="half" for="star3half" title="Meh - 3.5 stars"></label>
                                    <input onClick={() => setStarValue(3)} type="radio" id="star3" name="rating" value="3" /><label class="full" for="star3" title="Meh - 3 stars"></label>
                                    <input onClick={() => setStarValue(2.5)} type="radio" id="star2half" name="rating" value="2.5" /><label class="half" for="star2half" title="Kinda bad - 2.5 stars"></label>
                                    <input onClick={() => setStarValue(2)} type="radio" id="star2" name="rating" value="2" /><label class="full" for="star2" title="Kinda bad - 2 stars"></label>
                                    <input onClick={() => setStarValue(1.5)} type="radio" id="star1half" name="rating" value="1.5" /><label class="half" for="star1half" title="Meh - 1.5 stars"></label>
                                    <input onClick={() => setStarValue(1)} type="radio" id="star1" name="rating" value="1" /><label class="full" for="star1" title="Sucks big time - 1 star"></label>
                                    <input onClick={() => setStarValue(0.5)} type="radio" id="starhalf" name="rating" value="0.5" /><label class="half" for="starhalf" title="Sucks big time - 0.5 stars"></label>
                                </fieldset>
                            </div>
                            {starValue > 0 && (
                                <h4 style={{ textAlign: 'center', color: starValue < 3 ? 'red' : 'green' }}>Puan: {starValue}</h4>
                            )}
                            <Form.Floating className="mb-3">
                                <Form.Control
                                    id="commentTitle"
                                    placeholder="name@example.com"
                                    onChange={handleCommentTitle}
                                />
                                <label htmlFor="commentTitle">Yorum Başlığı</label>
                            </Form.Floating>
                            <Form.Floating className="mb-6">
                                <Form.Control
                                    id="comments"
                                    as="textarea"
                                    placeholder="Leave a comment here"
                                    onChange={handleCommentContext}
                                    style={{ height: '50vh', overflowY: 'auto' }}
                                />
                                <label htmlFor="comments">Yorumunuz</label>
                            </Form.Floating>
                            <Row>
                                <Col onClick={handleBack} style={{ textAlign: 'center' }}><Button className="mt-3" style={{ width: '10vw' }}>Geri</Button></Col>
                                <Col onClick={() => setShowModal(true)} style={{ textAlign: 'center' }}><Button className="mt-3" style={{ width: '10vw' }}>Yorumu gönder</Button></Col>
                            </Row>
                        </Col>
                    </Row>
                </div>
            )}
            <ToastContainer
                className="p-3"
                position='top-end'
                style={{ zIndex: 1 }}
            >
                <Toast onClose={() => setToast(prev => ({ ...prev, show: false }))} show={toast.show} delay={3000} autohide>
                    <Toast.Header closeButton={false}>
                        <strong className="me-auto">{toast.title}</strong>
                    </Toast.Header>
                    <Toast.Body>{toast.message}</Toast.Body>
                </Toast>
            </ToastContainer>
            <Modal show={showModal} onHide={() => setShowModal(false)} centered>
                <Modal.Header closeButton>
                    <Modal.Title>Yorum gönderiliyor</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    Yorum gönderilecek, emin misiniz?
                </Modal.Body>
                <Modal.Footer>
                    <Button onClick={() => setShowModal(false)}>İptal</Button>
                    <Button onClick={sendReview}>Onayla</Button>
                </Modal.Footer>
            </Modal>
        </div>
    );
}

export default Reviewer;