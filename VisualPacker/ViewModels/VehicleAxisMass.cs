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
            for (int a = Vehicle.AvtoAxisQuantity; a < vehicle.TrailerAxisQuantity; a++)
            {
                axisMassMeanList.Add(CargoTrailerAxiisMassCalculate());
            }
            return axisMassMeanList;
        }
        private void CargoAvtoMassCalculate()
        {
            double cargoAvtoMass = (((vehicle.EmptyTrailerTonnage + cargoTonnage) * 0.25) + vehicle.EmptyAvtoTonnage) / Vehicle.AvtoAxisQuantity;
            double cargoAvtoMassFirstAxis = CargoAvtoMassFirstAxisCalculate(cargoAvtoMass);
            double cargoAvtoMassSecondAxis = CargoAvtoMassSecondAxisCalculate(cargoAvtoMass, cargoAvtoMassFirstAxis);

            axisMassMeanList.Add(cargoAvtoMassFirstAxis);
            axisMassMeanList.Add(cargoAvtoMassSecondAxis);
        }
        private double CargoAvtoMassFirstAxisCalculate(double cargoAvtoMass)
        {
            return (cargoAvtoMass * 0.75) / Vehicle.AvtoAxisQuantity;
        }
        private double CargoAvtoMassSecondAxisCalculate(double cargoAvtoMass, double cargoAvtoMassFirstAxis)
        {
            return cargoAvtoMass-(cargoAvtoMassFirstAxis*2);
        }
        private double CargoTrailerAxiisMassCalculate()
        {
            return ((vehicle.EmptyTrailerTonnage + cargoTonnage) * 0.75) / vehicle.TrailerAxisQuantity;

        }
    }
}