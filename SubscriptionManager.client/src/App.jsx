import { BrowserRouter, Routes, Route } from 'react-router-dom'
import Login from './pages/Login'
import Register from './pages/Register'
import Subscriptions from './pages/Subscriptions'

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <Route path="/subscriptions" element={<Subscriptions />} />
      </Routes>
    </BrowserRouter>
  )
}

export default App