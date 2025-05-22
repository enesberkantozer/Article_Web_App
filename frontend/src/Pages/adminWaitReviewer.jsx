import axios from "axios";
import React, { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import { Row, Col, Button, Form, Card, ListGroup, Nav, ToastContainer, Toast, Modal, Spinner } from "react-bootstrap";

const AdminWaitReviewer = () => {
  const { articleId } = useParams();
  const [status, setStatus] = useState("loading");
  const [tabStatus, setTabStatus] = useState("#hakem");
  const [article, setArticle] = useState({});
  const [articleBlob, setArticleBlob] = useState(null);
  const [interests, setInterests] = useState([]);
  const [reviewers, setReviewers] = useState([]);
  const [sendReviewers, setSendReviewers] = useState([]);
  const [toast, setToast] = useState({ title: "", message: "", show: false });
  const [topics, setTopics] = useState([]);
  const [showTopicModal, setShowTopicModal] = useState(false);
  const [anonym, setAnonym] = useState({ name: false, company: false, contact: false });
  const [isLoading, setIsLoading] = useState(false);
  const [areUSure,setAreUSure] = useState(false);

  useEffect(() => {
    axios
      .get(
        "http://localhost:5253/api/Admin/GetWithId/" +
        encodeURIComponent(articleId)
      )
      .then((response) => {
        setArticle(response.data);
        setSendReviewers(response.data.reviewers);
        axios
          .get(
            "http://localhost:5253/uploads/" +
            encodeURIComponent(response.data.trackingCode) +
            "/original.pdf",
            {
              responseType: "blob",
            }
          )
          .then((response) => {
            let blobFile = URL.createObjectURL(response.data);
            setArticleBlob(blobFile);
          })
          .catch((error) => {
            console.error(error);
            console.log("File setup error");
            setToast({ title: "Hata", message: "Makaleler pdfi yüklenemedi", show: true });
          });
        handleTopics(response.data.id);
      })
      .catch((error) => {
        console.error(error.response);
        setToast({ title: "Hata", message: "Makaleler getirilemedi", show: true });
      });
    axios
      .get("http://localhost:5253/api/Reviewers/GetAllInterests")
      .then((response) => {
        setInterests(response.data);
        console.log(response.data);
      })
      .catch((error) => {
        console.error(error);
      });
    setStatus("completed");
  }, [articleId]);

  const handleReviewerList = async (e) => {
    e.preventDefault();
    setReviewers([]);
    axios
      .get(
        "http://localhost:5253/api/Reviewers/GetWithInterest/" + encodeURIComponent(articleId) + "/" + encodeURIComponent(e.target.value)
      )
      .then((response) => {
        console.log(response.data);
        setReviewers(response.data);
        setReviewers(prev => prev.filter(element => !sendReviewers.some(element1 => element1.id === element.id)));
      })
      .catch((error) => {
        console.error(error);
      });
  };

  const handleReviewerListDelete = async (id, name, mail) => {
    setSendReviewers(prev => [...prev, { id, name, mail }]);
    setReviewers(prev => prev.filter(element => element.id !== id));
  }

  const handleSendReviewerListDelete = async (id, name, mail) => {
    setReviewers(prev => [...prev, { id, name, mail }]);
    setSendReviewers(prev => prev.filter(element => element.id !== id));
  }

  const addReviewerToArticle = async (e) => {
    e.preventDefault();

    setAreUSure(false);
    setIsLoading(true);
    axios.put("http://localhost:5253/api/Services/setAnonymPdf", {
      articleId: articleId,
      hideName: anonym.name,
      hideCompany: anonym.company,
      hideMailPhone: anonym.contact
    }).then(() => {
      axios.put("http://localhost:5253/api/Reviewers/AddReviewersToArticle", {
        reviewers: sendReviewers.map(reviewer => reviewer.id),
        articleId: articleId
      }).then(response => {
        console.log(response.data);
        setSendReviewers([]);
        setIsLoading(false);
        setToast({title:"Başarılı",message:"Güncelleme yapıldı",show:true});
      }).catch(error => {
        console.error(error);
        setToast({ title: "Hata", message: "Hakeme gönderilemedi", show: true });
        setIsLoading(false);
        setAreUSure(true);
      });
    }).catch(error => {
      console.error(error);
      setToast({ title: "Uyarı", message: "Anonimleştirmeyi tekrar deneyiniz", show: true });
      setIsLoading(false);
      setAreUSure(true);
    });
  }

  const prepareAnonym = async () => {
    setIsLoading(true);
    axios.put("http://localhost:5253/api/Services/setAnonymPdf", {
      articleId: articleId,
      hideName: anonym.name,
      hideCompany: anonym.company,
      hideMailPhone: anonym.contact
    }).then(response => {
      axios.get("http://localhost:5253/uploads/" + encodeURIComponent(response.data) + "/anonym.pdf",
        {
          responseType: 'blob'
        }).then(response => {
          URL.revokeObjectURL(articleBlob);
          let blobFile = URL.createObjectURL(response.data);
          setArticleBlob(blobFile);
          setToast({ title: "Başarılı", message: "Anonimleştirme tamamlandı", show: true });
          setIsLoading(false);
        }).catch(error => {
          console.error(error);
          setToast({ title: "Uyarı", message: "Anonimleştirme başarılı ama önizleme getirilemedi", show: true });
          setIsLoading(false);
        });
    }).catch(error => {
      console.error(error);
      setToast({ title: "Uyarı", message: "Anonimleştirmeyi tekrar deneyiniz", show: true });
      setIsLoading(false);
    });
  }

  const handleTopics = async (id) => {
    axios.get("http://localhost:5253/api/Services/getTopics/" + encodeURIComponent(id))
      .then(response => {
        console.log(response.data);
        setTopics(response.data);
      }).catch(error => {
        console.error(error);
      });
  }

  const containerStyle = {
    display: "flex",
    justifyContent: "center",
    alignItems: "center",
    height: "100vh",
    background: "linear-gradient(90deg, #0f0c29 0%, #302b63 50%, #24243e 100%)",
  };

  const divStylePanel = {
    marginTop: "8vh",
    width: "96%",
    height: "88%",
    padding: "10px",
    border: "1px solid #ccc",
    borderRadius: "25px",
    background: "#fff",
  };

  return (
    <div style={containerStyle}>
      <div style={divStylePanel}>
        {status === "completed" && (
          <Row style={{ height: "100%" }}>
            <Col style={{ height: "100%", borderRight: "1px solid blue" }}>
              <iframe
                src={articleBlob}
                style={{
                  height: "100%",
                  width: "100%",
                  border: "1px solid black",
                }}
                title="Makale"
              ></iframe>
            </Col>
            <Col style={{ height: "100%" }}>
              <Row><Col>ID: {article.id}</Col><Col><h3 style={{ textAlign: "center" }}>{article.title}</h3></Col><Col></Col></Row>
              <Card className="text-center" style={{ height: "70vh" }}>
                <Card.Header>
                  <Nav fill variant="tabs" defaultActiveKey={tabStatus}>
                    <Nav.Item>
                      <Nav.Link
                        href="#hakem"
                        onClick={() => setTabStatus("#hakem")}
                      >
                        Hakemler
                      </Nav.Link>
                    </Nav.Item>
                    <Nav.Item>
                      <Nav.Link
                        href="#anonim"
                        onClick={() => setTabStatus("#anonim")}
                      >
                        Anonim
                      </Nav.Link>
                    </Nav.Item>
                  </Nav>
                </Card.Header>
                <Card.Body style={{ overflowY: "auto" }}>
                  {tabStatus === "#hakem" && (
                    <>
                      <Form.Select onChange={handleReviewerList}>
                        <option value="-1">İlgi Alanları</option>
                        {interests.map((interest) => (
                          <option value={interest.id}>{interest.topic}</option>
                        ))}
                      </Form.Select>
                      {(reviewers.length > 0 || sendReviewers.length > 0) && (
                        <>
                          <p>Hakemlere ekleme-çıkarma yapmak için çift tıkla</p>
                          <Row style={{ marginTop: "20px" }}>
                            <Col style={{ borderRight: "1px solid blue" }}>
                              <h5>Hakemler</h5>
                              <ListGroup>
                                {reviewers.map((reviewer) => (
                                  <ListGroup.Item
                                    as="li"
                                    action
                                    variant="secondary"
                                    onDoubleClick={() => handleReviewerListDelete(reviewer.id, reviewer.name, reviewer.mail)}
                                  >
                                    <h6 style={{ margin: "0px" }}>
                                      {reviewer.name}
                                    </h6>
                                    <p style={{ margin: "0px" }}>
                                      {reviewer.mail}
                                    </p>
                                  </ListGroup.Item>
                                ))}
                              </ListGroup>
                            </Col>
                            <Col>
                              <h5>Paylaşılan Hakemler</h5>
                              <ListGroup>
                                {sendReviewers.map((reviewer) => (
                                  <ListGroup.Item
                                    as="li"
                                    action
                                    variant="success"
                                    onDoubleClick={() => handleSendReviewerListDelete(reviewer.id, reviewer.name, reviewer.mail)}
                                  >
                                    <h6 style={{ margin: "0px" }}>
                                      {reviewer.name}
                                    </h6>
                                    <p style={{ margin: "0px" }}>
                                      {reviewer.mail}
                                    </p>
                                  </ListGroup.Item>
                                ))}
                              </ListGroup>
                            </Col>
                          </Row>
                        </>
                      )}
                    </>
                  )}

                  {tabStatus === "#anonim" && (
                    <>
                      <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', width: '100%', height: '100%', textAlign: 'left', fontSize: '20px' }}>
                        <Form onSubmit={prepareAnonym}>
                          <Form.Check
                            type="switch"
                            id="name-switch"
                            label="Kişi İsimleri"
                            checked={anonym.name}
                            onChange={async (e) => setAnonym(prev => ({ ...prev, name: e.target.checked }))}
                          />
                          <Form.Check
                            type="switch"
                            id="company-switch"
                            label="Firma İsimleri"
                            checked={anonym.company}
                            onChange={async (e) => setAnonym(prev => ({ ...prev, company: e.target.checked }))}
                          />
                          <Form.Check
                            type="switch"
                            id="contact-switch"
                            label="E-Posta Telefon Numarası"
                            checked={anonym.contact}
                            onChange={async (e) => setAnonym(prev => ({ ...prev, contact: e.target.checked }))}
                          />
                          <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', marginTop: '20px' }}>
                            <Button style={{ width: '50%' }} type="submit">Ön İzleme</Button>
                          </div>
                        </Form>
                      </div>
                    </>
                  )}
                </Card.Body>
              </Card>
              <div className="d-grid gap-2">
                <Row className="mt-3">
                  <Col>
                    <Button onClick={() => setShowTopicModal(true)} style={{ width: '50%', float: 'right', marginRight: '25px' }}>Anahtar Kelimeler</Button>
                  </Col>
                  <Col>
                    <Button onClick={()=>setAreUSure(true)} style={{ width: '50%', marginLeft: '25px' }}>Hakemlere yönlendir</Button>
                  </Col>
                </Row>
              </div>
            </Col>
          </Row>
        )}
      </div>
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
      <Modal show={showTopicModal} onHide={() => setShowTopicModal(false)} centered>
        <Modal.Header closeButton>
          <Modal.Title>Anahtar Kelimeler</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <ul>
            {topics.map(row => (
              <li style={{ textAlign: 'left' }}>{row[0]}</li>
            ))}
          </ul>
        </Modal.Body>
      </Modal>
      <Modal show={isLoading} centered>
        <Modal.Body>
          <div style={{ display: 'flex', flexDirection: 'column', width: '100%', height: '100%', justifyContent: 'center', alignItems: 'center' }}>
            <Spinner animation='grow' variant='primary' style={{ width: '5rem', height: '5rem' }} />
            <h2 style={{ textAlign: 'center' }}>Anonimleştiriliyor...</h2>
          </div>
        </Modal.Body>
      </Modal>
      <Modal show={areUSure} onHide={() => setAreUSure(false)} centered>
        <Modal.Header closeButton>
          <Modal.Title>Uyarı</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <h6>Hakem değişikliği yapılırken <u>anonim tekrar seçilmelidir</u></h6>
          <br />
          Bu ayarlarla kaydetmek istediğinize emin misiniz?
          <ul>
            <li>Kişi anonimleştirme : {anonym.name ? "Evet":"Hayır"}</li>
            <li>Firma anonimleştirme : {anonym.company ? "Evet":"Hayır"}</li>
            <li>Eposta-Telefon anonimleştirme : {anonym.contact ? "Evet":"Hayır"}</li>
          </ul>
        </Modal.Body>
        <Modal.Footer>
          <Button onClick={()=>setAreUSure(false)}>İptal</Button>
          <Button onClick={addReviewerToArticle}>Onayla</Button>
        </Modal.Footer>
      </Modal>
    </div>
  );
};

export default AdminWaitReviewer;
