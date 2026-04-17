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
        <div style={{ display: 'flex', flexDirection: 'column', justifyContent: 'center', alignItems: 'center', marginTop: '100px' }}>
            <div>
                <h1>Login</h1>
                <label>Email:</label>
                <input
                    type="email"
                    placeholder="Please enter your email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                />
            </div>
            <div>
                <label>Password:</label>
                <input
                    type="password"
                    placeholder="please enter your password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                />
            </div>
            <div>
                <button onClick={handleSubmit}>Submit</button>  <button onClick={registration}>Register</button>
            </div>
        </div>
    )
}

export default Login