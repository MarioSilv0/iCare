import { Injectable } from '@angular/core';
import { INGREDIENT_UNIT_WEIGHTS, MEASUREMENT_CONVERSION } from '../../data';
import { first } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MeasureConversionService {

  constructor() { }

  /**
   * Converte frações em número decimal de forma segura.
   */
  private parseFraction(fraction: string): number {
    if (fraction.includes('/')) {
      const [numerator, denominator] = fraction.split('/').map(Number);
      return denominator ? numerator / denominator : NaN;
    }
    return parseFloat(fraction);
  }

  /**
   * Remove o 's' do final da unidade de medida da primeira palavra, se presente.
   */
  private normalizeUnitFirst(unit: string): string {
    const parts = unit.split(" ");
    const firstWord = parts[0];
     
    if (firstWord.endsWith('s')) {
      if (firstWord.endsWith('colheres'))
        parts[0] = firstWord.slice(0, -2);
      else
        parts[0] = firstWord.slice(0, -1);
    }

    return parts.join(" "); // Reconstruindo a unidade
  }

  /**
   * Converte uma medida em string para gramas.
   */
  convertToGrams(measure: string, ingredient: string): number {

    const parts = measure.trim().toLowerCase().match(/^([\d\/.]+)\s*(.*)$/);

    let amount = 1;
    let unit = "";

    if (parts) {
      amount = this.parseFraction(parts[1]);
      unit = parts[2].trim().toLowerCase();
    } else {
      unit = measure.trim().toLowerCase();
    }

    unit = this.normalizeUnitFirst(unit);
    ingredient = this.normalizeUnitFirst(ingredient);

    if (!unit) {
      return INGREDIENT_UNIT_WEIGHTS[ingredient?.toLowerCase() || ""] * amount || amount;
    }

    return (MEASUREMENT_CONVERSION[unit] ?? 0) * amount;
  }

  /**
   * Adiciona peso em gramas a um ingrediente.
   */
  addWeightToIngredient(ingredient: { name: string; measure: string }) {
    return { ...ingredient, grams: this.convertToGrams(ingredient.measure, ingredient.name) };
  }

  /**
   * Processa uma lista de ingredientes adicionando o peso em gramas.
   */
  addWeightsToIngredients(ingredients: { name: string; measure: string }[]) {
    return ingredients.map(ingredient => this.addWeightToIngredient(ingredient));
  }
}
