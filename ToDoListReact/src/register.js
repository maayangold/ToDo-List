import React, { useState } from 'react';
import service from './service';
import './style.css';

function Register({ onRegister }) {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [registered, setRegistered] = useState(false);

    const handleRegister = async (e) => {
        e.preventDefault();
        try {
            await service.register({ username, password });
            setRegistered(true);
            onRegister();
            //alert('Registration successful!');
        } catch (error) {
            console.error('Registration failed:', error);
        }
    };

    if (registered) return null;

    return (
        <div className="container">
            <h2>Register</h2>
            <form onSubmit={handleRegister}>
                <div className="form-group">
                    <label className='label-style' htmlFor="username">Username:</label>
                    <input type="text" id="username" className="form-control" value={username} onChange={(e) => setUsername(e.target.value)} />
                </div>
                <div className="form-group">
                    <label className='label-style' htmlFor="password">Password:</label>
                    <input type="password" id="password" className="form-control" value={password} onChange={(e) => setPassword(e.target.value)} />
                </div>
                <button type="submit" className="btn btn-primary button-style">Register</button>
            </form>
        </div>
    );
}

export default Register;
