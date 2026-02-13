import { useEffect, useState } from "react";
import axios from "axios";

const API = "http://localhost:5171/api/claims";

export default function ClaimList({ reloadFlag }) {
  const [claims, setClaims] = useState([]);
  const [status, setStatus] = useState("");
  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [total, setTotal] = useState(0);

  const pageSize = 5;

  useEffect(() => {
    fetchData();
  }, [status, search, page, reloadFlag]);

  const fetchData = async () => {
    const res = await axios.get(API, {
      params: {
        status,
        search,
        page,
        pageSize
      }
    });

    setClaims(res.data.data);
    setTotal(res.data.totalCount);
  };
 const deleteClaim = async (id) => {
    await axios.delete(`${API}/${id}`);
    fetchData();
  };
  return (
    <div>
      <h2>Claim List</h2>

      {/* 🔎 Search */}
      <input
        placeholder="Search Policy or Customer"
        value={search}
        onChange={(e) => {
          setPage(1);
          setSearch(e.target.value);
        }}
      />

      {/* 🎯 Filter Status */}
      <select
        value={status}
        onChange={(e) => {
          setPage(1);
          setStatus(e.target.value);
        }}
      >
        <option value="">All</option>
        <option value="Pending">Pending</option>
        <option value="Approved">Approved</option>
        <option value="Rejected">Rejected</option>
      </select>

      <table border="1">
        <thead>
          <tr>
            <th>Policy</th>
            <th>Name</th>
            <th>Amount</th>
            <th>Status</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {claims.map(c => (
            <tr key={c.claimId}>
              <td>{c.policyNo}</td>
              <td>{c.customerName}</td>
              <td>{c.claimAmount}</td>
              <td>{c.status}</td>
              <td>
                <button onClick={() => deleteClaim(c.claimId)}>
                  Delete
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      <button
        disabled={page === 1}
        onClick={() => setPage(page - 1)}
      >
        Prev
      </button>

      <button
        disabled={page * pageSize >= total}
        onClick={() => setPage(page + 1)}
      >
        Next
      </button>
    </div>
  );
}
