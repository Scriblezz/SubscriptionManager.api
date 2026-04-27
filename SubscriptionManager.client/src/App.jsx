import { useState, useEffect } from 'react'
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'
import Login from './pages/Login'
import Register from './pages/Register'
import Subscriptions from './pages/Subscriptions'
import ProtectedRoute from './components/ProtectedRoute'

function App() {
  const [isDark, setIsDark] = useState(() => {
    return localStorage.getItem('darkMode') === 'true'
  })

  function toggleDark() {
    document.documentElement.classList.toggle('dark')
    setIsDark(prev => {
      localStorage.setItem('darkMode', !prev)
      return !prev
    })
  }

  useEffect(() => {
    if (isDark) {
      document.documentElement.classList.add('dark')
    }
  }, [])

  return (
    <div className={isDark ? 'dark' : ''}>
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<Navigate to="/login" />} />
          <Route path="/login" element={<Login toggleDark={toggleDark} isDark={isDark} />} />
          <Route path="/register" element={<Register toggleDark={toggleDark} isDark={isDark} />} />
          <Route path="/subscriptions" element={
            <ProtectedRoute>
              <Subscriptions toggleDark={toggleDark} isDark={isDark} />
            </ProtectedRoute>
          } />
        </Routes>
      </BrowserRouter>
    </div>
  )
}

export default App