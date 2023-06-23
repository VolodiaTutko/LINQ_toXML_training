using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

class Program_09_05
{
    public delegate void EventHandler<TEventArgs>(object? sender, TEventArgs e);

    static void Main(string[] args)

    {

        
    string dataFilePath = @"C:\Users\HP\source\repos\Tutko_09_05\Data.xml";
        Detector detector = new Detector(dataFilePath);

        PassengerTransportObserver passengerObserver = new PassengerTransportObserver(@"C:\Users\HP\source\repos\Tutko_09_05\PassengerViolation.xml");
        detector.SpeedViolation += passengerObserver.OnSpeedViolation;

        TruckTransportObserver truckObserver = new TruckTransportObserver(@"C:\Users\HP\source\repos\Tutko_09_05\TruckViolation.xml");
        detector.SpeedViolation += truckObserver.OnSpeedViolation;

        detector.Analyze();

        passengerObserver.GenerateXmlFile();
        truckObserver.GenerateXmlFile();
    }




    class Observation
    {
        public DateTime Timestamp { get; set; }
        public string LicensePlate { get; set; }
        public string VehicleCategory { get; set; }
        public int Speed { get; set; }
    }


    class Detector
    {
        public event EventHandler<Observation> SpeedViolation;

        private string filePath;

        public Detector(string filePath)
        {
            this.filePath = filePath;
        }

        public void Analyze()
        {

            List<Observation> observations = ReadObservationsFromFile();


            foreach (Observation observation in observations)
            {
                if (observation.Speed > 50)
                {

                    SpeedViolation?.Invoke(this, observation);
                }
            }
        }

        private List<Observation> ReadObservationsFromFile()
        {

            List<Observation> observations = new List<Observation>();

            using (XmlReader reader = XmlReader.Create(filePath))
            {
                reader.ReadToFollowing("observation");
                do
                {
                    Observation observation = new Observation();
                    reader.ReadToFollowing("datetime");
                    observation.Timestamp = DateTime.Parse(reader.ReadElementContentAsString());
                    reader.ReadToFollowing("license_plate");
                    observation.LicensePlate = reader.ReadElementContentAsString();
                    reader.ReadToFollowing("vehicle_category");
                    observation.VehicleCategory = reader.ReadElementContentAsString();
                    reader.ReadToFollowing("speed");
                    observation.Speed = int.Parse(reader.ReadElementContentAsString());
                    observations.Add(observation);
                } while (reader.ReadToFollowing("observation"));
            }

            return observations;

        }
    }

    // PASSENGER
    class PassengerTransportObserver
    {
        private string filePath;
        private List<Observation> p_violations = new List<Observation>();

        public PassengerTransportObserver(string filePath)
        {
            this.filePath = filePath;
        }

        public void OnSpeedViolation(object sender, Observation observation)
        {
            if (observation.VehicleCategory == "car" || observation.VehicleCategory == "bus")
            {
                p_violations.Add(observation);
            }
        }

        public void GenerateXmlFile()
        {
            using (XmlWriter writer = XmlWriter.Create(filePath))
            {
                writer.WriteStartElement("speed_violations");

                foreach (Observation violation in p_violations)
                {
                    writer.WriteStartElement("violation");
                    writer.WriteElementString("timestamp", violation.Timestamp.ToString("yyyy-MM-ddTHH:mm:ss"));
                    writer.WriteElementString("license_plate", violation.LicensePlate);
                    writer.WriteElementString("speed", violation.Speed.ToString());
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }
    }

    //TRUCK
    class TruckTransportObserver
    {
        private string filePath;
        private List<Observation> t_violations = new List<Observation>();

        public TruckTransportObserver(string filePath)
        {
            this.filePath = filePath;
        }

        public void OnSpeedViolation(object sender, Observation observation)
        {
            if (observation.VehicleCategory == "truck")
            {
                t_violations.Add(observation);
            }
        }

        public void GenerateXmlFile()
        {

            using (XmlWriter writer = XmlWriter.Create(filePath))
            {
                writer.WriteStartElement("speed_violations");

                foreach (Observation violation in t_violations)
                {
                    writer.WriteStartElement("violation");

                    writer.WriteElementString("timestamp", violation.Timestamp.ToString("yyyy-MM-ddTHH:mm:ss"));
                    writer.WriteElementString("license_plate", violation.LicensePlate);
                    writer.WriteElementString("speed", violation.Speed.ToString());

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();

            }








        }
    }
}
