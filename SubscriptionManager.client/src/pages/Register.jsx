import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import toast from 'react-hot-toast'

function Register({ toggleDark, isDark}) {
    const [email, setEmail] = useState('')
    const [password, setPassword] = useState('')
    const navigate = useNavigate()

    async function handleSubmit(e) {
        e.preventDefault();
        const response = await fetch(`${import.meta.env.VITE_API_URL}/api/Auth/register`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ email, password })
        })

        const data = await response.json()

        if (!response.ok) {
            toast.error(data.message || "Failed to Register")
            return
        }

        toast.success("Registration successful")
        navigate('/login')
    }
    
    return (
        <div className="relative flex flex-col items-center justify-center h-screen bg-gray-100 dark:bg-gray-900">
        <button
            onClick={toggleDark}
            className="absolute top-4 right-4 bg-gray-200 dark:bg-gray-700 dark:text-white px-3 py-1 rounded">
                {isDark ? 'Dark Mode' : 'Light Mode'}
            </button>
        <div className="bg-white dark:bg-gray-800 p-8 rounded shadow-md dark:text-white">
                <h1 className="text-3xl font-bold mb-6">Register</h1>
            <form onSubmit={handleSubmit}>
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
                    <input className="border border-gray-300 rounded px-3 py-2 w-64 focus:outline-none focus:border-blue-500"
                        type="password"
                        placeholder="Please enter your password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                    />
                </div>

                <div>
                    <button 
                    className="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600 w-full"
                    type="submit">Submit</button>
                </div>
            </form>
        </div>
        </div>
    )
}

export default Register