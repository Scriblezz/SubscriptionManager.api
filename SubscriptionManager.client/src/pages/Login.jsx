import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
function Login() {
    const [email, setEmail] = useState('')
    const [password, setPassword] = useState('')
    const [error, setError] = useState('');
    const navigate = useNavigate()
    async function handleSubmit(e) {
        e.preventDefault();
        setError('');
        const response = await fetch('http://localhost:5001/api/Auth/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ email, password })
        })

        const data = await response.json()

        if (!response.ok) {
            setError(data.message || 'Login failed');
            return;
        }
        localStorage.setItem('token', data.token)
        navigate('/subscriptions')
    }

    function registration() {
        navigate('/register')
    }
    return (
        
       <div className="flex flex-col items-center justify-center h-screen bg-gray-100">
        <div className="bg-white p-8 rounded shadow-md">
                <h1 className="text-3xl font-bold mb-6">Login</h1>
                <div className="flex flex-col mb-4">
                <label className="mb-1 text-sm font-medium">Email:</label>
                <input  className="border border-gray-300 rounded px-3 py-2 w-64 focus:outline-none focus:border-blue-500"
                    type="email"
                    placeholder="Please enter your email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                />
            </div>
            <div className="flex flex-col mb-4">
                <label className="mb-1 text-sm font-medium">Password:</label>
                <input  className="border border-gray-300 rounded px-3 py-2 w-64 focus:outline-none focus:border-blue-500"
                    type="password"
                    placeholder="please enter your password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                />
            </div>
            <div className="flex gap-2 mt-4">
  <button 
    className="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600 w-full"
    onClick={handleSubmit}>
    Login
  </button>
  <button 
    className="bg-gray-500 text-white px-4 py-2 rounded hover:bg-gray-600 w-full"
    onClick={registration}>
    Register
  </button>
</div>
            </div>
        </div>
    )
}

export default Login