import { useState } from "react";
import axios from "axios";

const API = "http://localhost:5171/api/claims";

export default function ClaimForm({ onSuccess }) {
  const [form, setForm] = useState({
    policyNo: "",
    customerName: "",
    claimAmount: 0,
    status: "Pending"
  });

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    await axios.post(API, form);
    setForm({
      policyNo: "",
      customerName: "",
      claimAmount: 0,
      status: "Pending"
    });
    onSuccess();
    
  };

  return (
    <form onSubmit={handleSubmit}>
      <input
        name="policyNo"
        value={form.policyNo}
        onChange={handleChange}
      />

      <input
        name="customerName"
        value={form.customerName}
        onChange={handleChange}
      />

      <input
        type="number"
        name="claimAmount"
        value={form.claimAmount}
        onChange={handleChange}
      />

      <select
        name="status"
        value={form.status}
        onChange={handleChange}
      >
        <option value="Pending">Pending</option>
        <option value="Approved">Approved</option>
        <option value="Rejected">Rejected</option>
      </select>
      <button type="submit">Create</button>
    </form>
  );
}
