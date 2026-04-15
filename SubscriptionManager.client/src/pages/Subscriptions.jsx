import { useState, useEffect } from "react"
function Subscriptions() {
  const [subscriptions, setSubscriptions] = useState([]);
  async function getSubscriptions() {
    const token = localStorage.getItem("token");
    const response = await fetch('http://localhost:5001/api/Subscriptions', {
      method: "GET",
      headers: {
        Authorization: `Bearer ${token}`
      }
    })

    if (!response.ok) {
      throw new Error("Failed to fetch subscriptions");
    }

    const data = await response.json();
    setSubscriptions(data.items);
  }

  useEffect(() => {
    getSubscriptions();
  }, [])

  return (
    <div>
      <h1>Subscriptions</h1>
      <ul>
        {subscriptions.map((sub) => (
          <li key={sub.id}>Name: {sub.name}, Price: ${sub.price}, Category: {sub.category}, Billing Cycle: {sub.billingCycle}</li>
        ))}
      </ul>
    </div>
  )
}

export default Subscriptions