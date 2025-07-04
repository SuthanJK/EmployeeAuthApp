import { useEffect, useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import "../App.css";

function Dashboard() {
  const [users, setUsers] = useState([]);
  const navigate = useNavigate();

  useEffect(() => {
    const token = localStorage.getItem('token');
    if (!token) return navigate('/');

    axios.get('https://localhost:7185/api/users', {
      headers: {
        Authorization: `Bearer ${token}`
      }
    })
    .then(res => setUsers(res.data))
    .catch(err => {
      console.error(err);
      navigate('/');
    });
  }, []);

  // Helper to decode JWT and get username
  function parseJwt (token) {
    try {
      const base64Url = token.split('.')[1];
      const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      const jsonPayload = decodeURIComponent(atob(base64).split('').map(function(c) {
        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
      }).join(''));
      return JSON.parse(jsonPayload);
    } catch {
      return {};
    }
  }

  const token = localStorage.getItem('token');
  const currentUsername = token ? parseJwt(token).unique_name || parseJwt(token).sub : null;

  // Group users by role, excluding current user
  const groupByRole = (users) => {
    return users.reduce((acc, user) => {
      if (user.username === currentUsername) return acc; // skip current user
      if (!acc[user.role]) acc[user.role] = [];
      acc[user.role].push(user);
      return acc;
    }, {});
  };

  const grouped = groupByRole(users);

  return (
    <div className="centered-container">
      <div className="dashboard-card">
        <h2>Dashboard</h2>
        {["SuperAdmin", "Admin", "Employee"].map(role => (
          grouped[role] && grouped[role].length > 0 && (
            <div key={role} style={{ marginBottom: "2rem" }}>
              <h3>{role}s</h3>
              <ul className="user-list">
                {grouped[role].map(u => (
                  <li key={u.id} className="user-list-item">
                    <span className="user-name">{u.username}</span>
                    <span className="user-role">{u.role}</span>
                  </li>
                ))}
              </ul>
            </div>
          )
        ))}
      </div>
    </div>
  );
}

export default Dashboard;
