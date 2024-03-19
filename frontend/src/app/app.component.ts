import { Component } from '@angular/core';
import { CalculoCdbService } from './_services/calculo-cdb.service';
import { CalculoCdbDto } from './_models/calculo-cdb-dto.model';
import { CalculoCdb } from './_models/calculo-cdb.model'

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {

  valorInicial: number = 0.0;
  quantidadeDeMeses: number = 0;

  valorFormatado: string = '';
  mesesFormatado: string = '';

  calculoResposta!: CalculoCdbDto;
  formatador: Intl.NumberFormat;

  constructor(private servico: CalculoCdbService) {
    this.formatador = new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL',
      
    })
  }

  formataValorInicial(input: InputEvent) {
    if (input.data) {
      this.valorFormatado += input.data?.replace(/\D/g, '');
      const formattedAmount = this.formatador.format(Number(this.valorFormatado) / 100);
      (input.target as HTMLInputElement).value = formattedAmount;
      return;
    }

    this.valorFormatado = '';
    (input.target as HTMLInputElement).value = this.valorFormatado;
  }

  formataQuantidadeDeMeses(input: InputEvent) {
    if (input.data) {
      this.mesesFormatado += input.data?.replace(/\D/g, '');
    } else {
      this.mesesFormatado = '';
    }

    (input.target as HTMLInputElement).value = this.mesesFormatado;
  }

  registraValorInicial(valor: string) {
    const valorNumerico = valor
      .replace('R$', '')
      .replace('.', '')
      .replace(',', '.');
    this.valorInicial = parseFloat(valorNumerico);
  }

  registraQuantidadeDeMeses(valor: string) {
    this.quantidadeDeMeses = parseInt(valor);
  }

  calcular() {
    const calculoCdb: CalculoCdb = {
      valorInicial: this.valorInicial,
      quantidadeDeMeses: this.quantidadeDeMeses
    };

    this.servico.calculaCdb(calculoCdb)
      .subscribe(resposta => this.calculoResposta = resposta);
  }
}
