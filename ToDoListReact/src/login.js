import React, { useState } from 'react';
import service from './service';
import './style.css';

function Login({ setShowRegister }) {
    // Pass setShowRegister as a prop
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');

    const handleLogin = async (e) => {
        e.preventDefault();
        try {
            const response = await service.login({ username, password });
            console.log('Logged in successfully!', response);
           // Store the token in local storage
            localStorage.setItem('token', response.token);

            window.location.reload(); // Reload the page to fetch todos with the new JWT token
        } catch (error) {
            // Handle login error
            console.error('Login failed:', error);
            if (error.response && error.response.status === 400) {
                setShowRegister(true);
                // Use setShowRegister to show the register component
            } else {
                setUsername('');
                setPassword('');
            }
        }
    };

    return (
        <div className="container">
            <h2>Login</h2>
            <form onSubmit={handleLogin}>
                <div className="form-group">
                    <label className='label-style' htmlFor="username">Username:</label>
                    <input type="text" id="username" className="form-control" value={username} onChange={(e) => setUsername(e.target.value)} />
                </div>
                <div className="form-group">
                    <label className='label-style' htmlFor="password">Password:</label>
                    <input type="password" id="password" className="form-control" value={password} onChange={(e) => setPassword(e.target.value)} />
                </div>
                <button type="submit" className="btn btn-primary button-style">Login</button>
            </form>
        </div>
    );
}

export default Login;
