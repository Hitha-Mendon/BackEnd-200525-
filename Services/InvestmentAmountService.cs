using System;
using Microsoft.Extensions.Logging;
using WisVestAPI.Constants;

namespace WisVestAPI.Services
{
    public class InvestmentAmountService
    {
        private readonly ILogger<InvestmentAmountService> _logger;

        private const double BASE_PERCENTAGE = 100.0;
        private const double BASE_RATE = 1.0;

        public InvestmentAmountService(ILogger<InvestmentAmountService> logger)
        {
            _logger = logger;
        }

        public double CalculateInvestmentAmount(double percentageSplit, double targetAmount, double annualReturn, int investmentHorizon)
        {
            try
            {
                // Validate inputs
                if (percentageSplit <= 0 || percentageSplit > BASE_PERCENTAGE)
                    throw new ArgumentException(ResponseMessages.InvalidPercentageSplit);

                if (targetAmount <= 0)
                    throw new ArgumentException(ResponseMessages.InvalidTargetAmount);

                if (annualReturn < 0)
                    throw new ArgumentException(ResponseMessages.InvalidAnnualReturn);

                if (investmentHorizon <= 0 )
                    throw new ArgumentException(ResponseMessages.InvalidInvestmentHorizon);

                _logger.LogInformation(ResponseMessages.CalculationStarted, percentageSplit, targetAmount, annualReturn, investmentHorizon);

                double denominator = Math.Pow((BASE_RATE + (annualReturn / BASE_PERCENTAGE)), investmentHorizon);
                if (denominator == 0)
                    throw new DivideByZeroException(ResponseMessages.DenominatorZero);

                double investmentAmount = ((percentageSplit / BASE_PERCENTAGE) * targetAmount) / denominator;
                investmentAmount = Math.Round(investmentAmount, 2);

                _logger.LogInformation(Constants.ResponseMessages.CalculationCompleted, investmentAmount);
                return investmentAmount;
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, ResponseMessages.InvalidInputLog, ex.Message);
                throw new ArgumentException(ResponseMessages.InvalidInputError, ex);
            }
            catch (DivideByZeroException ex)
            {
                _logger.LogError(ex, ResponseMessages.DivideByZeroLog, ex.Message);
                throw new DivideByZeroException(ResponseMessages.DivideByZeroError, ex);
            }
            catch (OverflowException ex)
            {
                _logger.LogError(ex, ResponseMessages.OverflowLog, ex.Message);
                throw new OverflowException(ResponseMessages.OverflowError, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ResponseMessages.UnexpectedErrorLog, ex.Message);
                throw new InvalidOperationException(ResponseMessages.UnexpectedError, ex);
            }
        }
    }
}
