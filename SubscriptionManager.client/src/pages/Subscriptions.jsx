import { useState, useEffect } from "react"
function Subscriptions() {
  const [subscriptions, setSubscriptions] = useState([]);
  const [name, setName] = useState('')
  const [price, setPrice] = useState('')
  const [category, setCategory] = useState('')
  const [billingCycle, setBillingCycle] = useState('')
  const [showForm, setShowForm] = useState(false);
  async function getSubscriptions() {
    const token = localStorage.getItem("token");
    const response = await fetch('http://localhost:5001/api/Subscriptions', {
      method: "GET",
      headers: {
        Authorization: `Bearer ${token}`
      }
    })

    if (!response.ok) {
      throw new Error("Unable to add Subscription")
    }

    const data = await response.json();
    setSubscriptions(data.items);
  }

  async function addSubscription(e) {
    e.preventDefault();
    const token = localStorage.getItem("token");
    const response = await fetch("http://localhost:5001/api/Subscriptions", {
      method: "POST",
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`
      },
      body: JSON.stringify({ name, price, category, billingCycle })
    })

    const data = await response.json()

    if (!response.ok) {
      throw new Error("Unable to add Subscription")
      return
    }

    setName('');
    setPrice('');
    setCategory('');
    setBillingCycle('');

    toggleField()
    getSubscriptions();
  }

async function deleteSubscription(id){
  const token = localStorage.getItem("token");
  const response = await fetch(`http://localhost:5001/api/Subscriptions/${id}`, {
    method: "DELETE",
    headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`
    }
  })


  if (!response.ok)
  {
    throw new Error("Unable to delete Subscription")
  }

  getSubscriptions();
}
  function toggleField() {
    setShowForm(prev => !prev);
  }

  useEffect(() => {
    getSubscriptions();
  }, [])

  return (
    <div>
      <h1>Subscriptions</h1>
      <ul style={{ display: 'flex', flexDirection: 'column', justifyContent: 'center', alignItems: 'center', marginTop: '100px' }}>
        {subscriptions.map((sub) => (
        <div key={sub.id}>
          <li>Streaming Service: {sub.name}, Price: ${sub.price}, Category: {sub.category}, Billing Cycle: {sub.billingCycle} </li>
          <button onClick={() => deleteSubscription(sub.id)}>Delete</button>
        </div>
        ))}
      </ul>
      {showForm && (
        <form onSubmit={addSubscription}>
          <label>Streaming Service:</label>
          <input
            type="text"
            placeholder="Netflix"
            value={name}
            onChange={(e) => setName(e.target.value)}
          />

          <label>Price:</label>
          <input
            type="number"
            step="0.01"
            placeholder="14.99"
            value={price}
            onChange={(e) => setPrice(e.target.value)}
          />

          <label>Category:</label>
          <input
            type="text"
            placeholder="Entertainment"
            value={category}
            onChange={(e) => setCategory(e.target.value)}
          />

          <label>Billing Cycle:</label>
          <input
            type="text"
            placeholder="Monthly"
            value={billingCycle}
            onChange={(e) => setBillingCycle(e.target.value)}
          />

          <button type="submit">Save Subscription</button>
        </form>
      )}
      <button onClick={toggleField}>
        {showForm ? "Cancel" : "Add Subscription"}
      </button>
    </div>
  )
}

export default Subscriptions