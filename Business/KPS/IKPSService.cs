namespace Business.KPS {
    public interface IKPSService {
        bool Validate(string name, string surname, ulong TCKN, int birthOfYear);
    }
}