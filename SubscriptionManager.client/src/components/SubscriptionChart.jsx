import { getMonthlyCost } from '../utils/subscriptionUtils'
import { Bar } from 'react-chartjs-2'
import {
    Chart as ChartJS,
    CategoryScale,
    LinearScale,
    BarElement,
    Title,
    Tooltip,
    Legend
} from 'chart.js'
ChartJS.register(CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend)

function SubscriptionChart({ subscriptions }) {
    const chartData = subscriptions.map(sub => ({
        name: sub.name,
        monthlyCost: getMonthlyCost(sub),
        yearlyCost: sub.billingCycle === "Yearly" ? sub.price : getMonthlyCost(sub) * 12
    }));

    return (
        <Bar
            data={{
                labels: chartData.map(d => d.name),
                datasets: [
                    {
                        label: 'Monthly Cost',
                        data: chartData.map(d => d.monthlyCost),
                        backgroundColor: '#3b82f6'
                    },
                    {
                        label: 'Yearly Cost',
                        data: chartData.map(d => d.yearlyCost),
                        backgroundColor: '#10b981'
                    }
                ]
            }}
        />
    );
}

export default SubscriptionChart;