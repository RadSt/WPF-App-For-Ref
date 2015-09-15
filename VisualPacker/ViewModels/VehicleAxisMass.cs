using System.Collections.Generic;
using VisualPacker.Models;

namespace VisualPacker.ViewModels
{
    public class VehicleAxisMass
    {
        private Vehicle vehicle;
        private double cargoTonnage;
        private List<double> axisMassMeanList = new List<double>();
        public VehicleAxisMass(Vehicle vehicle, double cargoTonnage)
        {
            this.vehicle = vehicle;
            this.cargoTonnage = cargoTonnage;
        }
        public List<double> AxisMassCalculate()
        {
            CargoAvtoMassCalculate();
            for (int a = Vehicle.AvtoAxisQuantity; a < Vehicle.AvtoAxisQuantity+vehicle.TrailerAxisQuantity; a++)
            {
                axisMassMeanList.Add(CargoTrailerAxiisMassCalculate());
            }
            return axisMassMeanList;
        }
        private void CargoAvtoMassCalculate()
        {
            double cargoAvtoMass = (((vehicle.EmptyTrailerTonnage + cargoTonnage)*0.25) + vehicle.EmptyAvtoTonnage);
            double cargoAvtoMassSecondAxis = CargoAvtoMassSecondAxisCalculate(cargoAvtoMass);
            double cargoAvtoMassFirstAxis = CargoAvtoMassFirstAxisCalculate(cargoAvtoMass, cargoAvtoMassSecondAxis);

            axisMassMeanList.Add(cargoAvtoMassFirstAxis);
            axisMassMeanList.Add(cargoAvtoMassSecondAxis);
        }
        private double CargoAvtoMassSecondAxisCalculate(double cargoAvtoMass)
        {
            return (cargoAvtoMass * 0.75) / Vehicle.AvtoAxisQuantity;
        }
        private double CargoAvtoMassFirstAxisCalculate(double cargoAvtoMass, double cargoAvtoMassFirstAxis)
        {
            return cargoAvtoMass-(cargoAvtoMassFirstAxis*2);
        }
        private double CargoTrailerAxiisMassCalculate()
        {
            return ((vehicle.EmptyTrailerTonnage + cargoTonnage) * 0.75) / vehicle.TrailerAxisQuantity;

        }
    }
}