import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { Button, Table, Form, ToastContainer, Toast, Modal, Nav, Card } from "react-bootstrap";
import { format } from "date-fns";
import axios from "axios";

const Admin = () => {
    const [checkProcessArticle, setcheckProcessArticle] = useState([]);
    const [waitingReviewersCommentArticle, setWaitingReviewersCommentArticle] = useState([]);
    const [waitingAdminApproveArticle, setWaitingAdminApproveArticle] = useState([]);
    const [completedArticle, setCompletedArticle] = useState([]);
    const [logs, setLogs] = useState([]);
    const [tabStatus, setTabStatus] = useState("#checking");
    const [status, setStatus] = useState("loading");
    const [toast, setToast] = useState({ title: "", message: "", show: false });
    const [showDeleteModal, setShowDeleteModal] = useState({ id: 0, show: false });
    const [pdfInfo, setpdfInfo] = useState({ data: null, title: null, trackingCode: null, id: 0, type: null, show: false });
    const navigate = useNavigate();

    useEffect(() => {
        axios.get("http://localhost:5253/api/Admin/GetCheckProcessArticles")
            .then(response => {
                setcheckProcessArticle(response.data);
                setStatus("completed");
            }).catch(error => {
                console.error(error);
                setToast({ title: "Uyarı", message: "Veriler çekilemedi", show: true });
            });
    }, []);

    const updateArticle = async (article) => {
        axios.put("http://localhost:5253/api/Admin/update", {
            id: article.id,
            status: article.status,
            isChangeable: article.isChangeable
        }).then(response => {
            setcheckProcessArticle(response.data);
            setToast({ title: "Başarılı", message: "Değişiklik uygulandı", show: true });
        }).catch(error => {
            console.error(error);
            setToast({ title: "Uyarı", message: "Makale güncelleme başarısız", show: true });
        });
    };


    const handleEditChange = async (checkId) => {
        const article = checkProcessArticle.find(a => a.id === checkId);
        if (article.status === "Revision" || article.status === "Checking") {
            if (article.status === "Revision")
                article.status = "Checking";
            else if (article.status === "Checking")
                article.status = "Revision";
            article.isChangeable = !(article.isChangeable);
            await updateArticle(article);
        }
        else {
            setToast({ title: "Uyarı", message: "Makale " + article.status + " haldeyken değişime izin verilmez", show: true });
        }
    };

    const handleDeleteArticle = async (id) => {
        axios.delete("http://localhost:5253/api/Admin/delete/" + encodeURIComponent(id))
            .then(() => {
                setToast({ title: "Başarılı", message: "Başarıyla silindi", show: true });
                setShowDeleteModal({ id: 0, show: false });
            }).catch(error => {
                console.error(error);
                setToast({ title: "Hata", message: "Makale silinemedi. Tekrar deneyiniz", show: true });
            });
        getTable(tabStatus);
    }

    const getTable = async (tabStatus) => {
        setTabStatus(tabStatus);

        if (tabStatus === "#checking") {
            axios.get("http://localhost:5253/api/Admin/GetCheckProcessArticles")
                .then(response => {
                    setcheckProcessArticle(response.data);
                }).catch(error => {
                    console.error(error);
                    setToast({ title: "Uyarı", message: "Veriler çekilemedi", show: true });
                });
        }
        else if (tabStatus === "#waitingReviewComment") {
            axios.get("http://localhost:5253/api/Admin/GetWaitingReviewersComment")
                .then(response => {
                    setWaitingReviewersCommentArticle(response.data);
                }).catch(error => {
                    console.error(error);
                    setToast({ title: "Uyarı", message: "Veriler çekilemedi", show: true });
                });
        }
        else if (tabStatus === "#waitingAdminApprove") {
            axios.get("http://localhost:5253/api/Admin/GetWaitingAdminApprove")
                .then(response => {
                    setWaitingAdminApproveArticle(response.data);
                }).catch(error => {
                    console.error(error);
                    setToast({ title: "Uyarı", message: "Veriler çekilemedi", show: true });
                });
        }
        else if (tabStatus === "#completed") {
            axios.get("http://localhost:5253/api/Admin/GetCompletedArticles")
                .then(response => {
                    setCompletedArticle(response.data);
                }).catch(error => {
                    console.error(error);
                    setToast({ title: "Uyarı", message: "Veriler çekilemedi", show: true });
                });
        }
        else if (tabStatus === "#logs") {
            axios.get("http://localhost:5253/api/Admin/GetLogs")
                .then(response => {
                    setLogs(response.data);
                }).catch(error => {
                    console.error(error);
                    setToast({ title: "Uyarı", message: "Veriler çekilemedi", show: true });
                });
        }
    }

    const preparepdf = async (id, title, trackingCode, type) => {
        axios.get("http://localhost:5253/uploads/"
            + encodeURIComponent(trackingCode) + "/" + encodeURIComponent(type) + ".pdf", {
            responseType: "blob"
        }).then(response => {
            let tempPdfUrl = URL.createObjectURL(response.data);
            setpdfInfo({ id: id, trackingCode: trackingCode, data: tempPdfUrl, title: title, type: type, show: true });
        }).catch(error => {
            console.error(error);
        });
    }

    const hidePdfModal = async () => {
        URL.revokeObjectURL(pdfInfo.data);
        setpdfInfo({ id: 0, trackingCode: null, data: null, title: null, type: null, show: false });
    }

    const addReviewsToOriginal = async (id) =>{
        axios.put("http://localhost:5253/api/Admin/AddReviewsToOriginal/"+encodeURIComponent(id))
            .then(()=>{
                setToast({title:"Başarılı",message:"Yazara başarıyla gönderildi",show:true});
            }).catch(error=>{
                console.error(error);
            });
    }

    const containerStyle = {
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        height: "100vh",
        background: "linear-gradient(90deg, #0f0c29 0%, #302b63 50%, #24243e 100%)"
    };

    const divStylePanel = {
        marginTop: '8vh',
        width: "96%",
        height: "88%",
        padding: "20px",
        border: "1px solid #ccc",
        borderRadius: "25px",
        background: "#fff"
    };


    return (
        <div style={containerStyle}>
            <div style={divStylePanel}>
                {status === "completed" && (
                    <Card>
                        <Card.Header>
                            <Nav fill variant="tabs" defaultActiveKey={tabStatus}>
                                <Nav.Item>
                                    <Nav.Link href="#checking" onClick={() => getTable("#checking")}>
                                        Hakem Yönlendirmesi Bekleyenler
                                    </Nav.Link>
                                </Nav.Item>
                                <Nav.Item>
                                    <Nav.Link href="#waitingReviewComment" onClick={() => getTable("#waitingReviewComment")} >
                                        Hakem Yorum Yapması Beklenenler
                                    </Nav.Link>
                                </Nav.Item>
                                <Nav.Item>
                                    <Nav.Link href="#waitingAdminApprove" onClick={() => getTable("#waitingAdminApprove")}>
                                        Yazara Yönlendirme Bekleyenler
                                    </Nav.Link>
                                </Nav.Item>
                                <Nav.Item>
                                    <Nav.Link href="#completed" onClick={() => getTable("#completed")}>
                                        Tamamlanmışlar
                                    </Nav.Link>
                                </Nav.Item>
                                <Nav.Item>
                                    <Nav.Link href="#logs" onClick={() => getTable("#logs")}>
                                        Log Kayıtları
                                    </Nav.Link>
                                </Nav.Item>
                            </Nav>
                        </Card.Header>
                        <Card.Body style={{ height: '77vh', overflowY: 'auto' }}>
                            {tabStatus === "#checking" && (
                                <Table striped responsive hover style={{ maxWidth: '100%', maxHeight: '100%' }}>
                                    <thead>
                                        <tr>
                                            <th>#</th>
                                            <th>Id</th>
                                            <th>Makale Başlığı</th>
                                            <th>Mail</th>
                                            <th>Yüklenme</th>
                                            <th>Güncelleme</th>
                                            <th>Revizyon İzni</th>
                                            <th>Durum</th>
                                            <th style={{ textAlign: 'center' }}>İşlemler</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {checkProcessArticle.map((row, index) => (
                                            <tr key={index}>
                                                <td>{index + 1}</td>
                                                <td>{row.id}</td>
                                                <td>{row.title}</td>
                                                <td>{row.authorMail}</td>
                                                <td>{format(new Date(row.uploadedTime), "dd.MM.yyyy HH:mm")}</td>
                                                <td>{format(new Date(row.updateTime), "dd.MM.yyyy HH:mm")}</td>
                                                <td>
                                                    <Form>
                                                        <Form.Check
                                                            type="switch"
                                                            checked={row.isChangeable}
                                                            onClick={(e) => handleEditChange(row.id)}
                                                            disabled={(row.status === "Shared" || row.status === "completed")} />
                                                    </Form>
                                                </td>
                                                <td>{row.status}</td>
                                                <td style={{ display: 'flex', justifyContent: 'center' }}>
                                                    <Button onClick={() => navigate("/yonetici/article/check/" + encodeURIComponent(row.id))} style={{ marginRight: '10px' }}>İşlem Yap</Button>
                                                    <Button variant="danger" onClick={() => setShowDeleteModal({ id: row.id, show: true })}>Sil</Button>
                                                </td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </Table>
                            )}
                            {tabStatus === "#waitingReviewComment" && (
                                <Table striped responsive hover style={{ maxWidth: '100%', maxHeight: '100%' }}>
                                    <thead>
                                        <tr>
                                            <th>#</th>
                                            <th>Id</th>
                                            <th>Makale Başlığı</th>
                                            <th>Mail</th>
                                            <th>Yüklenme</th>
                                            <th>Güncelleme</th>
                                            <th>Durum</th>
                                            <th style={{ textAlign: 'center' }}>İşlemler</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {waitingReviewersCommentArticle.map((row, index) => (
                                            <tr key={index}>
                                                <td>{index + 1}</td>
                                                <td>{row.id}</td>
                                                <td>{row.title}</td>
                                                <td>{row.authorMail}</td>
                                                <td>{format(new Date(row.uploadedTime), "dd.MM.yyyy HH:mm")}</td>
                                                <td>{format(new Date(row.updateTime), "dd.MM.yyyy HH:mm")}</td>
                                                <td>{row.status}</td>
                                                <td style={{ display: 'flex', justifyContent: 'center' }}>
                                                    <Button onClick={() => navigate("/yonetici/article/waitReviewer/" + encodeURIComponent(row.id))} style={{ marginRight: '10px' }}>İşlem Yap</Button>
                                                    <Button variant="danger" onClick={() => setShowDeleteModal({ id: row.id, show: true })}>Sil</Button>
                                                </td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </Table>
                            )}
                            {tabStatus === "#waitingAdminApprove" && (
                                <Table striped responsive hover style={{ maxWidth: '100%', maxHeight: '100%' }}>
                                    <thead>
                                        <tr>
                                            <th>#</th>
                                            <th>Id</th>
                                            <th>Makale Başlığı</th>
                                            <th>Mail</th>
                                            <th>Yüklenme</th>
                                            <th>Güncelleme</th>
                                            <th>Durum</th>
                                            <th style={{ textAlign: 'center' }}>İşlemler</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {waitingAdminApproveArticle.map((row, index) => (
                                            <tr key={index}>
                                                <td>{index + 1}</td>
                                                <td>{row.id}</td>
                                                <td>{row.title}</td>
                                                <td>{row.authorMail}</td>
                                                <td>{format(new Date(row.uploadedTime), "dd.MM.yyyy HH:mm")}</td>
                                                <td>{format(new Date(row.updateTime), "dd.MM.yyyy HH:mm")}</td>
                                                <td>{row.status}</td>
                                                <td style={{ display: 'flex', justifyContent: 'center' }}>
                                                    <Button onClick={() => preparepdf(row.id, row.title, row.trackingCode, "anonym")} style={{ marginRight: '10px' }}>İşlem Yap</Button>
                                                    <Button variant="danger" onClick={() => setShowDeleteModal({ id: row.id, show: true })}>Sil</Button>
                                                </td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </Table>
                            )}
                            {tabStatus === "#completed" && (
                                <Table striped responsive hover style={{ maxWidth: '100%', maxHeight: '100%' }}>
                                    <thead>
                                        <tr>
                                            <th>#</th>
                                            <th>Id</th>
                                            <th>Makale Başlığı</th>
                                            <th>Mail</th>
                                            <th>Yüklenme</th>
                                            <th>Güncelleme</th>
                                            <th>Durum</th>
                                            <th style={{ textAlign: 'center' }}>İşlemler</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {completedArticle.map((row, index) => (
                                            <tr key={index}>
                                                <td>{index + 1}</td>
                                                <td>{row.id}</td>
                                                <td>{row.title}</td>
                                                <td>{row.authorMail}</td>
                                                <td>{format(new Date(row.uploadedTime), "dd.MM.yyyy HH:mm")}</td>
                                                <td>{format(new Date(row.updateTime), "dd.MM.yyyy HH:mm")}</td>
                                                <td>{row.status}</td>
                                                <td style={{ display: 'flex', justifyContent: 'center' }}>
                                                    <Button onClick={() => preparepdf(row.id, row.title, row.trackingCode, "original")} style={{ marginRight: '10px' }}>Görüntüle</Button>
                                                    <Button variant="danger" onClick={() => setShowDeleteModal({ id: row.id, show: true })}>Sil</Button>
                                                </td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </Table>
                            )}
                            {tabStatus === "#logs" && (
                                <Table striped responsive hover style={{ maxWidth: '100%', maxHeight: '100%' }}>
                                    <thead>
                                        <tr>
                                            <th>#</th>
                                            <th>Id</th>
                                            <th>Türü</th>
                                            <th>Zaman</th>
                                            <th>Log Mesajı</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {logs.map((row, index) => (
                                            <tr key={index} style={{ backgroundColor: row.type === "Başarılı" ? "green" : "orange" }}>
                                                <td>{index + 1}</td>
                                                <td>{row.id}</td>
                                                <td>{row.type}</td>
                                                <td>{format(new Date(row.createdAt), "dd.MM.yyyy HH:mm:SS")}</td>
                                                <td>{row.logMessage}</td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </Table>
                            )}
                        </Card.Body>
                    </Card>

                )}
            </div>
            <Modal show={showDeleteModal.show} onHide={() => setShowDeleteModal({ id: 0, show: false })} centered>
                <Modal.Header closeButton>
                    <Modal.Title>Makale Silme Onayı</Modal.Title>
                </Modal.Header>
                <Modal.Body>İşlem geri alınamaz</Modal.Body>
                <Modal.Footer>
                    <Button onClick={() => setShowDeleteModal({ id: 0, show: false })}>İptal</Button>
                    <Button onClick={() => handleDeleteArticle(showDeleteModal.id)} variant="danger">Onayla</Button>
                </Modal.Footer>
            </Modal>
            <Modal show={pdfInfo.show} onHide={hidePdfModal} size="lg" centered>
                <Modal.Header closeButton>
                    <Modal.Title>{pdfInfo.title}.pdf</Modal.Title>
                </Modal.Header>
                <Modal.Body style={{ height: '70vh' }}>
                    <iframe src={pdfInfo.data} width='100%' height='100%' title="Makale"></iframe>
                </Modal.Body>
                {pdfInfo.type === "anonym" && (
                    <Modal.Footer>
                        <Button onClick={hidePdfModal}>İptal</Button>
                        <Button onClick={()=>addReviewsToOriginal(pdfInfo.id)}>Yazara Anonimsiz Gönder</Button>
                    </Modal.Footer>
                )}
            </Modal>
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
        </div>
    );
}

export default Admin;