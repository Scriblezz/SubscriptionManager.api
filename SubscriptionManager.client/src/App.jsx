import { useState } from 'react'
import { BrowserRouter, Routes, Route } from 'react-router-dom'
import Login from './pages/Login'
import Register from './pages/Register'
import Subscriptions from './pages/Subscriptions'

function App() {
  const [isDark, setIsDark] = useState(false)

  function toggleDark() {
    document.documentElement.classList.toggle('dark')
    setIsDark(prev => !prev)
  }
  return (
    <div className={isDark ? 'dark' : ''}>
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<Login toggleDark={toggleDark} isDark={isDark} />} />
        <Route path="/register" element={<Register toggleDark={toggleDark} isDark={isDark} />} />
        <Route path="/subscriptions" element={<Subscriptions toggleDark={toggleDark} isDark={isDark}/>} />
      </Routes>
    </BrowserRouter>
    </div>
  )
}

export default App