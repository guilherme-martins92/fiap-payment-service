using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using fiap_payment_service.Constants;
using fiap_payment_service.Dtos;
using fiap_payment_service.Models;
using fiap_payment_service.Repositories;
using fiap_payment_service.Service;
using fiap_payment_service.Infrastructure.EventBridge;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace fiap_payment_service_tests.Service
{
    public class PaymentServiceTests
    {
        private readonly Mock<ILogger<PaymentService>> _loggerMock;
        private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
        private readonly Mock<IEventPublisher> _eventPublisherMock;
        private readonly PaymentService _paymentService;

        public PaymentServiceTests()
        {
            _loggerMock = new Mock<ILogger<PaymentService>>();
            _paymentRepositoryMock = new Mock<IPaymentRepository>();
            _eventPublisherMock = new Mock<IEventPublisher>();
            _paymentService = new PaymentService(
                _loggerMock.Object,
                _paymentRepositoryMock.Object,
                _eventPublisherMock.Object
            );
        }

        [Fact]
        public async Task CreatePaymentAsync_ShouldCreatePaymentAndPublishEvent()
        {
            // Arrange
            var dto = new CreatePaymentDto
            {
                OrderId = Guid.NewGuid(),
                VehicleId = Guid.NewGuid(),
                Description = "Test Payment",
                Amount = 100m
            };
            Payment createdPayment = null!;
            _paymentRepositoryMock
                .Setup(r => r.CreatePaymentAsync(It.IsAny<Payment>()))
                .ReturnsAsync((Payment p) => { createdPayment = p; return p; });
            _eventPublisherMock
                .Setup(e => e.PublishPaymentStatusEventAsync(dto.OrderId, It.IsAny<Guid>(), PaymentStatus.Pending))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _paymentService.CreatePaymentAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.OrderId, result.OrderId);
            Assert.Equal(dto.VehicleId, result.VehicleId);
            Assert.Equal(dto.Description, result.Description);
            Assert.Equal(dto.Amount, result.Amount);
            Assert.Equal(PaymentStatus.Pending, result.Status);
            _paymentRepositoryMock.Verify(r => r.CreatePaymentAsync(It.IsAny<Payment>()), Times.Once);
            _eventPublisherMock.Verify(e => e.PublishPaymentStatusEventAsync(dto.OrderId, result.Id, PaymentStatus.Pending), Times.Once);
        }

        [Fact]
        public async Task GetPaymentByIdAsync_ShouldReturnPayment()
        {
            // Arrange
            var id = Guid.NewGuid();
            var payment = new Payment
            {
                Id = id,
                OrderId = Guid.NewGuid(),
                VehicleId = Guid.NewGuid(),
                Description = "desc",
                Amount = 10,
                Status = PaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow
            };
            _paymentRepositoryMock.Setup(r => r.GetPaymentByIdAsync(id)).ReturnsAsync(payment);

            // Act
            var result = await _paymentService.GetPaymentByIdAsync(id);

            // Assert
            Assert.Equal(payment, result);
        }

        [Fact]
        public async Task GetAllPaymentsAsync_ShouldReturnPayments()
        {
            // Arrange
            var payments = new List<Payment>
            {
                new Payment
                {
                    Id = Guid.NewGuid(),
                    OrderId = Guid.NewGuid(),
                    VehicleId = Guid.NewGuid(),
                    Description = "desc",
                    Amount = 10,
                    Status = PaymentStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    UpdateAt = DateTime.UtcNow
                }
            };
            _paymentRepositoryMock.Setup(r => r.GetAllPaymentsAsync()).ReturnsAsync(payments);

            // Act
            var result = await _paymentService.GetAllPaymentsAsync();

            // Assert
            Assert.Equal(payments, result);
        }

        [Fact]
        public async Task UpdatePaymentAsync_ShouldUpdatePayment()
        {
            // Arrange
            var id = Guid.NewGuid();
            var existingPayment = new Payment
            {
                Id = id,
                OrderId = Guid.NewGuid(),
                VehicleId = Guid.NewGuid(),
                Description = "old",
                Amount = 1,
                Status = PaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow
            };
            var dto = new CreatePaymentDto
            {
                OrderId = existingPayment.OrderId,
                VehicleId = existingPayment.VehicleId,
                Description = "new",
                Amount = 2
            };
            _paymentRepositoryMock.Setup(r => r.GetPaymentByIdAsync(id)).ReturnsAsync(existingPayment);
            _paymentRepositoryMock.Setup(r => r.UpdatePaymentAsync(It.IsAny<Payment>())).ReturnsAsync((Payment p) => p);

            // Act
            var result = await _paymentService.UpdatePaymentAsync(id, dto);

            // Assert
            Assert.Equal(dto.Description, result.Description);
            Assert.Equal(dto.Amount, result.Amount);
            _paymentRepositoryMock.Verify(r => r.UpdatePaymentAsync(It.IsAny<Payment>()), Times.Once);
        }

        [Fact]
        public async Task DeletePaymentAsync_ShouldReturnTrue()
        {
            // Act
            var result = await _paymentService.DeletePaymentAsync(Guid.NewGuid());

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ProcessPaymentAsync_ShouldUpdateStatusAndPublishEvent()
        {
            // Arrange
            var id = Guid.NewGuid();
            var payment = new Payment
            {
                Id = id,
                OrderId = Guid.NewGuid(),
                VehicleId = Guid.NewGuid(),
                Description = "desc",
                Amount = 10,
                Status = PaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow
            };
            _paymentRepositoryMock.Setup(r => r.GetPaymentByIdAsync(id)).ReturnsAsync(payment);
            _paymentRepositoryMock.Setup(r => r.UpdatePaymentAsync(It.IsAny<Payment>())).ReturnsAsync((Payment p) => p);
            _eventPublisherMock.Setup(e => e.PublishPaymentStatusEventAsync(payment.OrderId, payment.Id, PaymentStatus.Completed)).Returns(Task.CompletedTask);

            // Act
            var result = await _paymentService.ProcessPaymentAsync(id);

            // Assert
            Assert.Equal(PaymentStatus.Completed, result.Status);
            _paymentRepositoryMock.Verify(r => r.UpdatePaymentAsync(It.IsAny<Payment>()), Times.Once);
            _eventPublisherMock.Verify(e => e.PublishPaymentStatusEventAsync(payment.OrderId, payment.Id, PaymentStatus.Completed), Times.Once);
        }
    }
}
