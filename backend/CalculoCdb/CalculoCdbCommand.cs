﻿using MediatR;

namespace WebApi.CalculoCdb;

public class CalculoCdbCommand : IRequest<CalculoCdbDto>
{
    public decimal ValorInicial { get; set; }
    public int QuantidaDeMeses { get; set; }
}
