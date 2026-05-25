  export function getMonthlyCost(sub) {
    if (sub.billingCycle === "Monthly") return sub.price;
    if (sub.billingCycle === "Yearly") return sub.price / 12;
    if (sub.billingCycle === "Weekly") return sub.price * 4.33;
    return sub.price;
  }
  