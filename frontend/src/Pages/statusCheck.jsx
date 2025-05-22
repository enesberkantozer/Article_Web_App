import React, { useState, useEffect } from "react";
import { Button, Modal, ProgressBar, Container, Row, Col, ToastContainer, Toast, Spinner, Form, OverlayTrigger, Tooltip } from "react-bootstrap";
import { useParams, useNavigate } from "react-router-dom";
import axios from "axios";
import { format } from "date-fns";

const StatusCheck = () => {

    const { tracking } = useParams();
    const [inputTrackingCode, setInputTrackingCode] = useState();
    const [progressBarValue, setProgressBarValue] = useState(0);
    const [article, setArticle] = useState({});
    const [status, setStatus] = useState("form");
    const [showTitleModal, setShowTitleModal] = useState(false);
    const [toast, setToast] = useState({ title: "", message: "", show: false });
    const [title, setTitle] = useState(null);
    const [pdfUrl, setPdfUrl] = useState(null);
    const [pdfModal, setPdfModal] = useState(false);
    const [newPdfFile, setNewPdfFile] = useState(null);
    const [showUpdatePdfModal, setShowUpdatePdfModal] = useState(false);
    const navigate = useNavigate();

    useEffect(() => {
        if (tracking) {
            setStatus("loading");
            axios.get("http://localhost:5253/api/Articles/" + encodeURIComponent(tracking))
                .then(response => {
                    console.log(response.data);
                    setArticle(response.data);
                    switch (response.data.status) {
                        case "Revision":
                            setProgressBarValue(1);
                            break;
                        case "Checking":
                            setProgressBarValue(2);
                            break;
                        case "Waiting Reviewers Comment":
                            setProgressBarValue(3);
                            break;
                        case "Waiting Admin Approve":
                            setProgressBarValue(4);
                            break;
                        case "Completed":
                            setProgressBarValue(5);
                            break;

                        default:
                            setProgressBarValue(0);
                            break;
                    }
                    setStatus("panel");
                })
                .catch(error => {
                    console.error(error);
                    setToast({ title: "Uyarı", message: "Böyle bir takip numarası bulunamadı", show: true });
                    navigate("/durumKontrol");
                    setStatus("form");
                });
        }
        else {
            setStatus("form");
        }
    }, [tracking, navigate]);

    const handleEditTitle = async (e) => {
        if (article.isChangeable) {
            if (showTitleModal)
                setShowTitleModal(false);
            else
                setShowTitleModal(true);
        }
        else {
            if (article.status === "Shared" || article.status === "Completed")
                setToast({ title: "Uyarı", message: "Bu aşamada değişiklik yapılamaz", show: true });
            else
                setToast({ title: "Uyarı", message: "Yöneticiden düzenleme izin isteyiniz", show: true });
        }
    };

    const handleConfirmTitleModal = async (e) => {
        e.preventDefault();

        if (article.isChangeable && title !== null) {
            axios.put("http://localhost:5253/api/Articles", {
                title: title,
                trackingCode: tracking,
                status: article.status,
                isChangeable: article.isChangeable,
                updateTime: new Date(Date.now()).toISOString()
            }).then(response => {
                setArticle(response.data);
                setToast({ title: "Başarılı", message: "Başlık başarıyla değiştirildi", show: true });
                setTitle(null);
                setShowTitleModal(false);
            }).catch(error => {
                console.error(error);
                setToast({ title: "Başarısız", message: "Tekrar deneyiniz", show: true });
            });
        }
        else {
            setToast({ title: "Uyarı", message: "Başlık girişi yapmadınız", show: true });
        }
    };

    const openPdfModal = async (e) => {
        e.preventDefault();
        URL.revokeObjectURL(pdfUrl);
        axios.get("http://localhost:5253/uploads/" + encodeURIComponent(tracking) + "/original.pdf", {
            responseType: 'blob'
        }).then(response => {
            let creatingPdfUrl = URL.createObjectURL(response.data);
            setPdfUrl(creatingPdfUrl);
        }).catch(error => {
            console.error(error);
            setToast({ title: "Uyarı", message: "Pdf dosyası çekilemedi", show: true });
        });
        setPdfModal(true);
    };

    const handleUpdatePdf = async (e) => {
        if (article.isChangeable) {
            if (showUpdatePdfModal)
                setShowUpdatePdfModal(false);
            else
                setShowUpdatePdfModal(true);
        }
        else {
            if (article.status === "Shared" || article.status === "Completed")
                setToast({ title: "Uyarı", message: "Bu aşamada değişiklik yapılamaz", show: true });
            else
                setToast({ title: "Uyarı", message: "Yöneticiden düzenleme izin isteyiniz", show: true });
        }
    };

    const updatePdf = async (e) => {
        e.preventDefault();

        if (newPdfFile !== null) {
            const isSuccessUpload = await uploadNewFile(tracking, newPdfFile);
            if (isSuccessUpload) {
                setNewPdfFile(null);
                setShowUpdatePdfModal(false);
                setToast({ title: "Başarılı", message: "Pdf başarıyla güncellendi", show: true });
            }
            else
                setToast({ title: "Başarısız", message: "Tekrar deneyiniz", show: true });
        }
        else
            setToast({ title: "Uyarı", message: "Dosya seçimi yapmadınız", show: true });
    };

    const uploadNewFile = async (trackingCode, file) => {
        const formData = new FormData();
        formData.append("TrackingCode", trackingCode);
        formData.append("File", file);

        return axios.put("http://localhost:5253/api/Articles/WithFile", formData, {
            headers: { "Content-Type": "multipart/form-data" }
        }).then(response => {
            return "Article updated successfully" === response.data;
        }).catch(error => {
            console.error(error);
            return false;
        });
    };

    const checkTrackingCode = async (e) => {
        e.preventDefault();
        navigate("/durumKontrol/" + encodeURIComponent(inputTrackingCode));
    };

    const handleTitleChange = (e) => {
        setTitle(e.target.value);
    };

    const handlePdfChange = (e) => {
        setNewPdfFile(e.target.files[0]);
    };

    const handleInputTrackingCode = (e) => {
        setInputTrackingCode(e.target.value);
    };

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
        width: "70%",
        height: "56%",
        padding: "20px",
        border: "1px solid #ccc",
        borderRadius: "25px",
        background: "#fff"
    };

    return (
        <div style={containerStyle}>
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
            {status === "form" && (
                <div style={divStyleForm}>
                    <Form onSubmit={checkTrackingCode} style={{ display: 'flex', flexDirection: 'column', justifyContent: 'center', alignItems: 'center', width: '100%', height: '100%' }}>
                        <h2>Makale Takip No</h2>
                        <Form.Group className="mb-4 mt-4" controlId="formBasicEmail" style={{ width: '100%' }}>
                            <Form.Control required onChange={handleInputTrackingCode} type="text" style={{ textAlign: 'center' }} />
                        </Form.Group>
                        <Button variant="primary" type="submit">
                            Onayla
                        </Button>
                    </Form>
                </div>
            )}

            {status === "loading" && (
                <div style={divStyleForm}>
                    <div style={{ display: 'flex', flexDirection: 'column', width: '100%', height: '100%', justifyContent: 'center', alignItems: 'center' }}>
                        <Spinner animation='grow' variant='primary' style={{ width: '5rem', height: '5rem' }} />
                        <h2 style={{ textAlign: 'center' }}>Yükleniyor...</h2>
                    </div>
                </div>
            )}

            {status === "panel" && (
                <div style={divStylePanel}>
                    <h2 style={{ textAlign: 'center', textDecorationLine: 'underline' }}>
                        {article.title}
                        <OverlayTrigger key="top" placement="top" overlay={
                            <Tooltip>Düzenle</Tooltip>
                        }>
                            <svg onClick={handleEditTitle} disabled={(article.status === "Shared" || article.status === "Completed")} style={{ width: "20px", height: "20px", marginLeft: "20px" }} xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512"><path fill="#74C0FC" d="M362.7 19.3L314.3 67.7 444.3 197.7l48.4-48.4c25-25 25-65.5 0-90.5L453.3 19.3c-25-25-65.5-25-90.5 0zm-71 71L58.6 323.5c-10.4 10.4-18 23.3-22.2 37.4L1 481.2C-1.5 489.7 .8 498.8 7 505s15.3 8.5 23.7 6.1l120.3-35.4c14.1-4.2 27-11.8 37.4-22.2L421.7 220.3 291.7 90.3z" /></svg>
                        </OverlayTrigger>
                    </h2>
                    <p>Sahibi: <u>{article.authorMail}</u></p>
                    <p>Takip No: {article.trackingCode}</p>
                    <p>Yüklenme Zamanı: {format(new Date(article.uploadedTime), "dd.MM.yyyy HH:mm")}</p>
                    <p>Son Güncellenme Zamanı: {format(new Date(article.updateTime), "dd.MM.yyyy HH:mm")}</p>
                    <p>Durum: {article.status}</p>
                    <ProgressBar variant="success" min={0} max={5} now={progressBarValue}></ProgressBar>
                    <Container>
                        <Row>
                            <Col></Col>
                            <Col>Revize İstendi</Col>
                            <Col><p style={{ textAlign: 'center' }}>İnceleniyor</p></Col>
                            <Col><p style={{ textAlign: 'center' }}>Hakem yorumu bekleniyor</p></Col>
                            <Col><p style={{ textAlign: 'right' }}>Admin yönlendirme onayı bekleniyor</p></Col>
                            <Col><p style={{ textAlign: 'right' }}>Tamamlandı</p></Col>
                        </Row>
                        <Row style={{ marginTop: '2em' }}>
                            <Col style={{ textAlign: 'right' }}>
                                <Button onClick={openPdfModal}>Pdf'i Görüntüle</Button>
                            </Col>
                            <Col>
                                <Button onClick={handleUpdatePdf} disabled={(article.status === "Shared" || article.status === "Completed")}>Pdf'i Güncelle</Button>
                            </Col>
                        </Row>
                    </Container>
                    <Modal show={showTitleModal} onHide={handleEditTitle} centered>
                        <Modal.Header closeButton>
                            <Modal.Title>Başlığı Düzenle</Modal.Title>
                        </Modal.Header>
                        <Modal.Body>
                            <Form>
                                <Form.Group className="mb-3" controlId="exampleForm.ControlInput1">
                                    <Form.Control required onChange={handleTitleChange} placeholder={title} type="text" autoFocus />
                                </Form.Group>
                            </Form>
                        </Modal.Body>
                        <Modal.Footer>
                            <Button onClick={handleConfirmTitleModal}>Onayla</Button>
                        </Modal.Footer>
                    </Modal>
                    <Modal show={pdfModal} onHide={() =>{setPdfModal(false); URL.revokeObjectURL(pdfUrl); setPdfUrl(null);}} size="lg" centered>
                        <Modal.Header closeButton>
                            <Modal.Title>{article.title}.pdf</Modal.Title>
                        </Modal.Header>
                        <Modal.Body style={{ height: '80vh' }}>
                            <iframe src={pdfUrl} width='100%' height='100%' title="Makale"></iframe>
                        </Modal.Body>
                    </Modal>
                    <Modal show={showUpdatePdfModal} onHide={handleUpdatePdf} centered>
                        <Modal.Header closeButton>
                            <Modal.Title>Pdf Yükle</Modal.Title>
                        </Modal.Header>
                        <Modal.Body>
                            <Form onSubmit={updatePdf} style={{ display: 'flex', flexDirection: 'column' }}>
                                <Form.Group controlId="formFile" className="mb-3">
                                    <Form.Control type="file" accept="application/pdf" onChange={handlePdfChange} required />
                                </Form.Group>
                                <Button type="submit">Yükle</Button>
                            </Form>
                        </Modal.Body>
                    </Modal>
                </div>
            )}
        </div>
    );
}

export default StatusCheck;