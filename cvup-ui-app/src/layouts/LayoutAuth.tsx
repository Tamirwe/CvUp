import { useEffect, useState } from "react";
import { Outlet } from "react-router-dom";

export const LayoutAuth: React.FC = () => {
  const [id, setId] = useState(0);

  useEffect(() => {}, []);

  return (
    <div>
      <div>Layout Auth</div>
      <Outlet />
    </div>
  );
};
