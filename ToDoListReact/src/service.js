import axios from 'axios';

const apiUrl = " http://localhost:5163"

export default {
  getTasks: async () => {
    const result = await axios.get(`${apiUrl}/tasks`)    
    return result.data;
  },
  addTask: async(name)=>{
    console.log('addTask', name)
    const result = await axios.post(`${apiUrl}/tasks`, { name });
      return result.data;
  },

  setCompleted: async(id, isComplete)=>{
    console.log('setCompleted', {id, isComplete})
    const result = await axios.put(`${apiUrl}/tasks/${id}`, { isComplete });
    return result.data;
  },

  deleteTask:async(id)=>{
    try {
      await axios.delete(`${apiUrl}/tasks/${id}`);
      console.log("Task deleted successfully.");
    } catch (error) {
      console.error("Error deleting task:", error);
      throw error;
    }
  }
};
