import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Spinner } from 'react-bootstrap';
import axios from 'axios';
import success from '../success.png';

const ArticleUpload = () => {
    const [email, setEmail] = useState("");
    const [title, setTitle] = useState("");
    const [pdf, setPdf] = useState(null);
    const [status, setStatus] = useState("form");
    const [trackingcode, setTrackingCode] = useState("");
    const [sendMailMessage, setSendMailMessage] = useState("");
    const [isCopied, setisCopied] = useState("Panoya Kopyala");
    const [progressTimer, setProgressTimer] = useState(180);
    const [percentage, setPercentage] = useState("100%");
    const [isTimerActive, setIsTimerActive] = useState(false);
    const [verificationCode, setVerificationCode] = useState(0);
    const navigate = useNavigate();

    const handleEmailChange = (e) => {
        setEmail(e.target.value);
    };

    const hnadleTitleChange = (e) => {
        setTitle(e.target.value);
    };

    const handlePdfChange = (e) => {
        setPdf(e.target.files[0]);
    };

    const handleNumberChange = (e) => {
        setVerificationCode(e.target.value);
    };

    const handleCopy = () => {
        navigator.clipboard.writeText(trackingcode);
        setisCopied("Kopyalandı!");
    };

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
            setStatus("form");
        }
        return () => clearInterval(interval);
    }, [isTimerActive, progressTimer]);

    const handleSubmit = async (e) => {
        e.preventDefault();

        setStatus("loading");
        const formData1 = new FormData();
        formData1.append("email", email);
        try {
            const mail = await axios.post('http://localhost:5253/api/Mails/sendVerification/' + encodeURIComponent(email));
            await mail.data;
            setStatus("verification");
            setIsTimerActive(true);
        } catch (error) {
            console.error("Hata:", error);
            setStatus("error");
            alert("Doğrulama kodu gönderilemedi!");
        }
    };

    const handleCheckVerification = async (e) => {
        e.preventDefault();
        // Handle form submission logic here
        if (!pdf) {
            alert("Lütfen bir dosya seçin!");
            return;
        }

        if (!(await checkCode(email, verificationCode))) {
            alert("Doğrulama kodu yanlış");
            return;
        }

        setIsTimerActive(false);
        setProgressTimer(180);
        setPercentage("100%");
        setStatus("loading");

        let trackNumber=await uploadFile(email,title,pdf);
        if(trackNumber<=0){
            setStatus("error");
            alert("Yükleme başarısız!");
        }
        setTrackingCode(trackNumber);

        if(await mailSend(email,trackNumber)){
            setSendMailMessage(<p>Mail başarıyla gönderildi.<br />En kısa zamanda değerlendirmeye alınacaktır!</p>);
        }
        else{
            setSendMailMessage(<p>Takip maili gönderilemedi fakat dosya gönderimi başarılı!<br />En kısa zamanda değerlendirmeye alınacaktır!</p>);
        }

        await deleteAllVerificationCode(email);

        setStatus("success");
        setEmail("");
        setTitle("");
        setPdf(null);
    };

    const checkCode = async (mail, code) => {
        return await axios.get('http://localhost:5253/api/Mails/check/' + encodeURIComponent(mail))
            .then(response =>{
                return (parseInt(code)===parseInt(response.data));
            })
            .catch(error =>{
                console.error(error);
                return false;
            });
    };

    const uploadFile = async (mail, title, file) => {
        const formData = new FormData();
        formData.append("file", file);
        formData.append("title", title);
        formData.append("email", mail);

        return axios.post("http://localhost:5253/api/Articles/upload", formData, {
            headers: { "Content-Type": "multipart/form-data" }
        }).then(response => {
            return response.data;
        }).catch(error =>{
            console.error(error);
            return -1;
        });
    };

    const mailSend = async (mail, code) => {
        return axios.post('http://localhost:5253/api/Mails/sendMail', {
            email: mail,
            subject: 'Makale Takip No',
            body: 'Makalenizin takip numarası: '+code
        }).then(response =>{
            return response.statusText==="OK";
        }).catch(error =>{
            console.error(error);
            return false;
        });
    };

    const deleteAllVerificationCode = async (mail) =>{
        await axios.delete('http://localhost:5253/api/Mails/deleteVerification/' + encodeURIComponent(email))
            .then(() =>{
                console.log("Successfully deleted verifications");
            }).catch(() =>{
                console.error("Verifications delete failed");
            });
    };

    const containerStyle = {
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        height: "100vh",
        background: "linear-gradient(90deg, #0f0c29 0%, #302b63 50%, #24243e 100%)"
    };
    const divStyle = {
        width: "40%",
        height: "50%",
        padding: "20px",
        border: "1px solid #ccc",
        borderRadius: "25px",
        background: "#fff"
    };
    return (
        <div style={containerStyle}>
            <div style={divStyle}>
                {(status === "form" || status === "error") && (
                    <>
                        <h2 style={{ width: '100%', textAlign: 'center' }}>Makale Yükleme</h2><br />
                        <form onSubmit={handleSubmit}>
                            <div class="form-group">
                                <label for="exampleInputEmail1">E-Mail:</label><br />
                                <input
                                    type="email"
                                    class="form-control"
                                    id="exampleInputEmail1"
                                    aria-describedby="emailHelp"
                                    placeholder="Email girişi yapın"
                                    required
                                    onChange={handleEmailChange}
                                />
                            </div>
                            <br />
                            <div class="form-group">
                                <label for="exampleInputEmail1">Makale Başlığı:</label><br />
                                <input
                                    type="text"
                                    class="form-control"
                                    id="exampleInputText1"
                                    aria-describedby="textHelp"
                                    placeholder="Makale başlığı girin"
                                    required
                                    onChange={hnadleTitleChange}
                                />
                            </div>
                            <br />
                            <div class="form-group">
                                <label for="exampleFormControlFile1">Dosya (Pdf):</label><br />
                                <input
                                    type="file"
                                    class="form-control-file"
                                    id="exampleFormControlFile1"
                                    accept="application/pdf"
                                    required
                                    onChange={handlePdfChange}
                                />
                            </div><br />
                            <button type="submit" class="btn btn-primary" style={{ textAlign: 'center', width: '100%' }}>Yükle</button>
                        </form>
                    </>
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

                {status === "loading" && (
                    <div style={{ display: 'flex', flexDirection: 'column', width: '100%', height: '100%', justifyContent: 'center', alignItems: 'center' }}>
                        <Spinner animation='grow' variant='primary' style={{width: '5rem', height: '5rem'}}/>
                        <h2 style={{ textAlign: 'center' }}>Yükleniyor...</h2>
                    </div>
                )}

                {status === "success" && (
                    <div style={{ textAlign: 'center' }}>
                        <img src={success} alt="sucess" height="150px" />
                        <h2 style={{ width: '100%', textAlign: 'center', color: "green" }}>Yükleme Başarılı</h2>
                        <label style={{ fontSize: "20px" }}>{sendMailMessage}</label>
                        <div class="input-group mb-3">
                            <input type="text" class="form-control" aria-describedby="basic-addon2" value={trackingcode} disabled />
                            <div class="input-group-append">
                                <button class="btn btn-outline-secondary" type="button" onClick={handleCopy} disabled={isCopied === "Kopyalandı!"}>
                                    {isCopied}
                                </button>
                            </div>
                        </div>
                        <div>
                            <button class="btn btn-primary" style={{ width: '40%', margin: "0px 20px" }} onClick={() => {
                                setStatus("form");
                                setEmail("");
                                setTitle("");
                                setPdf(null);
                                setTrackingCode("");
                                setisCopied("Panoya Kopyala");
                                setSendMailMessage("");
                            }}>
                                Yeni Makale Yükle
                            </button>
                            <button class="btn btn-primary" style={{ width: '40%', margin: "0px 20px" }} onClick={() => {
                                setTrackingCode("");
                                setisCopied("Panoya Kopyala");
                                setSendMailMessage("");
                                navigate("/durumKontrol/"+encodeURIComponent(trackingcode));
                            }}>
                                Durum Kontrol
                            </button>
                        </div>
                    </div>
                )}
            </div>
        </div>
    );
}

export default ArticleUpload;