import axios from 'axios';
//Config Defaults
const instance = axios.create({
  baseURL: ' http://localhost:5163'
});

// Adding interceptor for error handling
instance.interceptors.response.use(
  response => {
    return response;
  },
  error => {
    console.error('Oops! An error occurred:', error);
     if (error.response.status === 401) {
       window.location.href('/login'); // Redirect to login page
     }
    return Promise.reject(error);
  }
);

export default {
  getTasks: async () => {
    const result = await instance.get('/tasks')
    return result.data;
  },
  addTask: async (name) => {
    console.log('addTask', name)
    const result = await instance.post('/tasks', { name });
    return result.data;
  },

  setCompleted: async (id, isComplete) => {
    console.log('setCompleted', { id, isComplete })
    const result = await instance.put(`/tasks/${id}`, { isComplete });
    return result.data;
  },

  deleteTask: async (id) => {
    try {
      await instance.delete(`/tasks/${id}`);
      console.log("Task deleted successfully.");
    } catch (error) {
      console.error("Error deleting task:", error);
      throw error;
    }
  },
  login: async (credentials) => {
    try {
      const response = await instance.post(`/login`, credentials);
      return response.data;
    } catch (error) {
      throw error;
    }
  },
  checkLogin: () => {
    // Check if the token exists in local storage
    const token = localStorage.getItem('token');
    return !!token;
     // Convert to boolean value
  },

  // Method for user registration
  register: async (userData) => {
    try {
      const response = await instance.post(`/register`, userData);
      return response.data;
    } catch (error) {
      throw error;
    }
  }

};
