import React, { useEffect, useState } from "react";
import Loading from "../../components/loading";

interface Profile {
  id: number;
  vardas: string;
  pavarde: string;
  elektroninisPastas: string;
}

const Profiliai: React.FC = () => {
  const [profiles, setProfiles] = useState<Profile[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchProfiles = async () => {
      try {
        const response = await fetch("https://localhost:5210/api/Profilis");
        if (!response.ok) {
          throw new Error("Klaida gaunant profilį");
        }
        const data = await response.json();

        // Sort profiles by id in ascending order
        const sortedData = data.sort((a: Profile, b: Profile) => a.id - b.id);

        setProfiles(sortedData);
      } catch (err: any) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchProfiles();
  }, []);

  if (loading) {
    return <Loading />;
  }

  if (error) {
    return <div className="container mt-5 alert alert-danger">Klaida: {error}</div>;
  }

  return (
    <div className="container mt-5 mb-5">
      <h1 className="display-5 mb-4 text-center">Naudotojų profiliai</h1>
      <div className="table-responsive shadow rounded">
        <table className="table table-hover table-bordered align-middle">
          <thead className="table-success">
            <tr>
              <th className="text-center">Naudotojo ID</th>
              <th>Vardas</th>
              <th>Pavardė</th>
              <th>El. paštas</th>
              <th className="text-center">Veiksmai</th>
            </tr>
          </thead>
          <tbody>
            {profiles.map((profile) => (
              <tr key={profile.id}>
                <td className="text-center">{profile.id}</td>
                <td>{profile.vardas}</td>
                <td>{profile.pavarde}</td>
                <td>{profile.elektroninisPastas}</td>
                <td className="text-center">
                  <a
                    href={`/profilis?id=${profile.id}`}
                    className="btn btn-sm btn-outline-success"
                  >
                    Peržiūrėti
                  </a>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default Profiliai;
