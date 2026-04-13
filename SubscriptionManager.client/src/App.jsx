import { BrowserRouter, Routes, Route } from 'react-router-dom'

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<h1>Login Page</h1>} />
        <Route path="/register" element={<h1>Register Page</h1>} />
        <Route path="/subscriptions" element={<h1>Subscriptions Page</h1>} />
      </Routes>
    </BrowserRouter>
  )
}

export default App