import { useEffect, useState } from "react";
import { Outlet } from "react-router-dom";

export const LayoutNotAuth: React.FC = () => {
  const [id, setId] = useState(0);

  useEffect(() => {}, []);

  return (
    <div>
      <div>Layout Not Auth</div>
      <Outlet />
    </div>
  );
};
