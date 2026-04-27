import { useState, useEffect } from "react"
import { useNavigate } from "react-router-dom";
import toast from "react-hot-toast";

function Subscriptions({ toggleDark, isDark }) {
  const [subscriptions, setSubscriptions] = useState([]);
  const [name, setName] = useState('')
  const [price, setPrice] = useState('')
  const [category, setCategory] = useState('')
  const [billingCycle, setBillingCycle] = useState('')
  const [showForm, setShowForm] = useState(false);
  const [editingId, setEditingId] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const navigate = useNavigate()

  async function getSubscriptions() {
    setIsLoading(true);
    const token = localStorage.getItem("token");
    const response = await fetch(`${import.meta.env.VITE_API_URL}/api/Subscriptions`, {
      method: "GET",
      headers: {
        Authorization: `Bearer ${token}`
      }
    })
    if (!response.ok) {
      toast.error("Unable to fetch Subscriptions")
      return
    }
    const data = await response.json();
    setSubscriptions(data.items);
    setIsLoading(false);
  }

  async function handleSubmit(e) {
    e.preventDefault();
    const token = localStorage.getItem("token");

    if (editingId) {
      const response = await fetch(`${import.meta.env.VITE_API_URL}/api/Subscriptions/${editingId}`, {
        method: "PUT",
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`
        },
        body: JSON.stringify({ name, price, category, billingCycle })
      })
      if (!response.ok) {
        toast.error("Unable to edit Subscription")
        return
      }
      toast.success('Subscription updated!')
    } else {
      const response = await fetch(`${import.meta.env.VITE_API_URL}/api/Subscriptions`, {
        method: "POST",
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`
        },
        body: JSON.stringify({ name, price, category, billingCycle })
      })
      if (!response.ok) {
        toast.error("Unable to add Subscription")
        return
      }
      toast.success('Subscription added!')
    }

    setName('');
    setPrice('');
    setCategory('');
    setBillingCycle('');
    setEditingId(null);
    setShowForm(false);
    getSubscriptions();
  }

  async function deleteSubscription(id) {
    const token = localStorage.getItem("token");
    const response = await fetch(`${import.meta.env.VITE_API_URL}/api/Subscriptions/${id}`, {
      method: "DELETE",
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`
      }
    })
    if (!response.ok) {
      toast.error("Unable to delete Subscription")
      return
    }
    toast.success('Subscription deleted!')
    getSubscriptions();
  }

  async function renewSubscription(id) {
    const token = localStorage.getItem("token");
    const response = await fetch(`${import.meta.env.VITE_API_URL}/api/Subscriptions/${id}/renew`, {
      method: "POST",
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`
      }
    })
    if (!response.ok) {
      const data = await response.json()
      toast.error(data.message || "Unable to renew Subscription")
      return
    }
    toast.success('Subscription renewed!')
    getSubscriptions();
  }

  function startEdit(sub) {
    setEditingId(sub.id);
    setName(sub.name);
    setPrice(sub.price);
    setCategory(sub.category);
    setBillingCycle(sub.billingCycle);
    setShowForm(true);
  }

  function logout() {
    localStorage.removeItem('token')
    navigate("/login")
  }

  function toggleField() {
    setShowForm(prev => !prev);
    setName('');
    setPrice('');
    setCategory('');
    setBillingCycle('');
    setEditingId(null);
  }

  useEffect(() => {
    getSubscriptions();
  }, [])

  return (
    <div className="min-h-screen bg-gray-100 dark:bg-gray-900 dark:text-white p-8">
      <div className="flex justify-between items-center mb-8">
        <h1 className="text-3xl font-bold">Subscriptions</h1>
        <div className="flex gap-2">
          <button onClick={toggleDark} className="bg-gray-200 dark:bg-gray-700 dark:text-white px-3 py-1 rounded">
            {isDark ? 'Light Mode' : 'Dark Mode'}
          </button>
          <button onClick={logout} className="bg-red-500 text-white px-3 py-1 rounded hover:bg-red-600">
            Logout
          </button>
        </div>
      </div>

      {isLoading ? (
        <div className="flex justify-center mt-20">
          <div className="w-10 h-10 border-4 border-blue-500 border-t-transparent rounded-full animate-spin"></div>
        </div>
      ) : (
        <ul className="flex flex-col gap-4">
          {subscriptions.map((sub) => (
            <div key={sub.id} className="bg-white dark:bg-gray-800 p-4 rounded shadow flex justify-between items-center">
              <div>
                <p className="font-bold">{sub.name}</p>
                <p className="text-sm text-gray-500 dark:text-gray-400">${sub.price} · {sub.category} · {sub.billingCycle}</p>
                <p className="text-sm text-gray-500 dark:text-gray-400">Next Renewal: {new Date(sub.nextRenewalDate).toLocaleDateString("en-US")}</p>
              </div>
              <div className="flex gap-2">
                {sub.isActive && new Date(sub.nextRenewalDate) < new Date() && (
                  <button
                    onClick={() => renewSubscription(sub.id)}
                    className="bg-green-500 text-white px-3 py-1 rounded hover:bg-green-600">
                    Renew
                  </button>
                )}
                <button
                  onClick={() => startEdit(sub)}
                  className="bg-yellow-500 text-white px-3 py-1 rounded hover:bg-yellow-600">
                  Edit
                </button>
                <button
                  onClick={() => deleteSubscription(sub.id)}
                  className="bg-red-500 text-white px-3 py-1 rounded hover:bg-red-600">
                  Delete
                </button>
              </div>
            </div>
          ))}
        </ul>
      )}

      <div className="mt-6 flex justify-end">
        <button
          onClick={toggleField}
          className="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600 mb-4">
          {showForm ? "Cancel" : "Add Subscription"}
        </button>

        {showForm && (
          <div
            className="fixed inset-0 bg-black bg-opacity-50 z-40"
            onClick={toggleField}
          />
        )}
        <div className={`fixed right-0 top-0 h-full w-96 bg-white dark:bg-gray-800 dark:text-white shadow-xl z-50 transform transition-transform duration-300 ${showForm ? 'translate-x-0' : 'translate-x-full'}`}>
          <div className="p-6 flex flex-col gap-4">
            <div className="flex justify-between items-center mb-2">
              <h2 className="text-xl font-bold">{editingId ? 'Edit Subscription' : 'Add Subscription'}</h2>
              <button onClick={toggleField} className="text-gray-500 hover:text-gray-700 dark:text-gray-400">✕</button>
            </div>
            <form onSubmit={handleSubmit} className="flex flex-col gap-4">
              <div className="flex flex-col">
                <label className="mb-1 text-sm font-medium">Name:</label>
                <input className="border border-gray-300 rounded px-3 py-2 focus:outline-none focus:border-blue-500"
                  type="text" placeholder="Netflix" value={name}
                  onChange={(e) => setName(e.target.value)} />
              </div>
              <div className="flex flex-col">
                <label className="mb-1 text-sm font-medium">Price:</label>
                <input className="border border-gray-300 rounded px-3 py-2 focus:outline-none focus:border-blue-500"
                  type="number" step="0.01" placeholder="14.99" value={price}
                  onChange={(e) => setPrice(e.target.value)} />
              </div>
              <div className="flex flex-col">
                <label className="mb-1 text-sm font-medium">Category:</label>
                <input className="border border-gray-300 rounded px-3 py-2 focus:outline-none focus:border-blue-500"
                  type="text" placeholder="Entertainment" value={category}
                  onChange={(e) => setCategory(e.target.value)} />
              </div>
              <div className="flex flex-col">
                <label className="mb-1 text-sm font-medium">Billing Cycle:</label>
                <select
                  className="border border-gray-300 rounded px-3 py-2 focus:outline-none focus:border-blue-500 dark:bg-gray-700 dark:text-white dark:border-gray-600"
                  value={billingCycle}
                  onChange={(e) => setBillingCycle(e.target.value)}>
                  <option value="">Select a billing cycle</option>
                  <option value="Weekly">Weekly</option>
                  <option value="Monthly">Monthly</option>
                  <option value="Yearly">Yearly</option>
                </select>
              </div>
              <button type="submit" className="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600">
                Save Subscription
              </button>
            </form>
          </div>
        </div>
      </div>
    </div>
  )
}

export default Subscriptions