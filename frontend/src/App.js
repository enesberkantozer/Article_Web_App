import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import {BrowserRouter as Router,Routes,Route } from 'react-router-dom';
import Topbar from './Components/topbar';
import ArticleUpload from './Pages/articleUpload';
import StatusCheck from './Pages/statusCheck';
import Admin from './Pages/admin';
import AdminArticleControl from './Pages/adminArticleControl';
import AdminWaitReviewer from './Pages/adminWaitReviewer';
import Reviewer from './Pages/reviewer';

function App() {
  return (
    <Router>
      <Topbar />
      <Routes>
        <Route path="/makaleYukle" element={<ArticleUpload />} />
        <Route path="/durumKontrol/:tracking" element={<StatusCheck />} />
        <Route path="/durumKontrol" element={<StatusCheck />} />
        <Route path="/yonetici" element={<Admin />} />
        <Route path="/yonetici/article/check/:articleId" element={<AdminArticleControl />} />
        <Route path="/yonetici/article/waitReviewer/:articleId" element={<AdminWaitReviewer />} />
        <Route path="/hakem" element={<Reviewer />} />
      </Routes>
    </Router>
  );
}

export default App;
