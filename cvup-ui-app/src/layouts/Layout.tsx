import { useEffect, useState } from "react";
import { Outlet } from "react-router-dom";

export const Layout: React.FC = () => {
  const [id, setId] = useState(0);

  useEffect(() => {}, []);

  return (
    <div>
      <div>Layout</div>
      <Outlet />
    </div>
  );
};
