import { useEffect, useState } from "react";
import axios from "axios";

const API = "http://localhost:5171/api/claims";

const initialForm = {
  claimId: 0,
  policyNo: "",
  customerName: "",
  claimAmount: 0,
  status: "Pending"
};

export default function ClaimForm({ selectedClaim, onSuccess }) {
  const [form, setForm] = useState(initialForm);

  // 🔥 โหลดข้อมูลเข้า form ตอนกด edit
  useEffect(() => {
    if (selectedClaim) {
      setForm(selectedClaim);
    } else {
      setForm(initialForm);
    }
  }, [selectedClaim]);

  const handleChange = (e) => {
    setForm({
      ...form,
      [e.target.name]: e.target.value
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (form.claimId === 0) {
      // CREATE
      await axios.post(API, form);
    } else {
      // UPDATE
      await axios.put(`${API}/${form.claimId}`, form);
    }

    setForm(initialForm);
    onSuccess();
  };

  return (
    <form onSubmit={handleSubmit}>
      <input
        name="policyNo"
        value={form.policyNo}
        onChange={handleChange}
        placeholder="Policy No"
        required
      />

      <input
        name="customerName"
        value={form.customerName}
        onChange={handleChange}
        placeholder="Customer Name"
        required
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
        <option>Pending</option>
        <option>Approved</option>
        <option>Rejected</option>
      </select>

      <button type="submit">
        {form.claimId === 0 ? "Create" : "Update"}
      </button>
    </form>
  );
}
