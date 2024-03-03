import React, { useEffect, useState } from 'react';
import service from './service.js';
import Login from './login';
import Register from './register';

function App() {
  const [newTodo, setNewTodo] = useState("");
  const [todos, setTodos] = useState([]);
  const [loggedIn, setLoggedIn] = useState(false);
  const [showRegister, setShowRegister] = useState(false); // State to control whether to show the register component

  async function getTodos() {
    const todos = await service.getTasks();
    setTodos(todos);
  }

  async function createTodo(e) {
    e.preventDefault();
    await service.addTask(newTodo);
    setNewTodo(""); // Clear input
    await getTodos(); // Refresh tasks list (in order to see the new one)
  }

  async function updateCompleted(todo, isComplete) {
    await service.setCompleted(todo.id, isComplete);
    await getTodos(); // Refresh tasks list (in order to see the updated one)
  }

  async function deleteTodo(id) {
    await service.deleteTask(id);
    await getTodos(); // Refresh tasks list
  }

  const handleLogin = async ({ username, password }) => {
    try {
      await service.login({ username, password });
      setLoggedIn(true); // If login successful, set loggedIn to true
    } catch (error) {
      console.error('Login failed:', error);
      if (error.response && error.response.status) {
        setShowRegister(true); // Show register component only if login failed due to user not being signed up
        alert('Login failed: User not registered. Please register.'); // Use window.alert to show the alert
      }
    }
  };
  const handleRegister = () => {
    setShowRegister(false); // Hide the register component after successful registration
  };
  
  const handleLogout = () => {
    localStorage.removeItem('token'); // Clear token from local storage
    setLoggedIn(false); // Update state to reflect logged out status
    setTodos([]); // Clear todos
  };

  useEffect(() => {
    const checkLoginStatus = async () => {
      try {
        // Check login status
        const isLoggedIn = service.checkLogin();
        setLoggedIn(isLoggedIn); // Update state based on login status
        if (isLoggedIn) {
          await getTodos(); // Fetch todos if logged in
        }
      } catch (error) {
        console.error('Error checking login status:', error);
      }
    };

    checkLoginStatus();
  }, []);

  return (
    <section className="todoapp">
      <header className="header">
        <h1>todos</h1>
        {loggedIn && (
          <button className="btn btn-primary  button-style" onClick={handleLogout}>Logout</button>
        )}
        {!loggedIn && !showRegister && <Login onLogin={handleLogin} setShowRegister={setShowRegister} />}
        {(!loggedIn && showRegister)  && <Register onRegister={handleRegister} />} 
        {loggedIn && (
          <form onSubmit={createTodo}>
            <input className="new-todo" placeholder="Well, let's take on the day" value={newTodo} onChange={(e) => setNewTodo(e.target.value)} />
          </form>
        )}
      </header>
      {loggedIn && (
        <section className="main" style={{ display: "block" }}>
          <ul className="todo-list">
            {todos.map(todo => {
              return (
                <li className={todo.isComplete ? "completed" : ""} key={todo.id}>
                  <div className="view">
                    <input className="toggle" type="checkbox" defaultChecked={todo.isComplete} onChange={(e) => updateCompleted(todo, e.target.checked)} />
                    <label>{todo.name}</label>
                    <button className="destroy" onClick={() => deleteTodo(todo.id)}></button>
                  </div>
                </li>
              );
            })}
          </ul>
        </section>
      )}
    </section >
  );
}

export default App;
