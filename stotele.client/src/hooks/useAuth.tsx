import { useState, useEffect } from "react";

interface User {
  id: number;
  name: string;
  email: string;
  administratorius: boolean;
}

const useAuth = () => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [user, setUser] = useState<User | null>(null);

  useEffect(() => {
    const storedUser = localStorage.getItem("user");
    const token = localStorage.getItem("token");
  
    try {
      if (storedUser && token) {
        const parsedUser = JSON.parse(storedUser);
        setIsAuthenticated(true);
        setUser(parsedUser);
      } else {
        throw new Error("Invalid user or token data");
      }
    } catch (error) {
      console.error("Error parsing user data:", error);
      setIsAuthenticated(false);
      setUser(null);
    }
  }, []);
  

  const logout = () => {
    localStorage.removeItem("user");
    localStorage.removeItem("token");
    setIsAuthenticated(false);
    setUser(null);
  };

  return { isAuthenticated, user, logout };
};

export default useAuth;
