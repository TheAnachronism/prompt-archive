import axios from 'axios'

const api = axios.create({
    baseURL: import.meta.env.VITE_API_URL || 'https://localhost:7132/api/v1',
    headers: {
        'Content-Type': 'application/json'
    },
    withCredentials: true
})

export default api
