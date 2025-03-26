# Caminhos principais
$JMeterPath = "C:\Program Files\apache-jmeter-5.6.3\bin\jmeter.bat"
$ProjectPath = (Get-Location).Path  # Diretório onde o script está localizado

# Load Test
$LoadTestPlan = "$ProjectPath\performance_tests\local_load_tests.jmx"
$LoadResultsFile = "$ProjectPath\performance_tests\local_load_results.jtl"
$LoadReportDir = "$ProjectPath\performance_tests\local_load_report"

# Stress Test
$StressTestPlan = "$ProjectPath\performance_tests\local_stress_tests.jmx"
$StressResultsFile = "$ProjectPath\performance_tests\local_stress_results.jtl"
$StressReportDir = "$ProjectPath\performance_tests\local_stress_report"

function Run-JMeterTest {
    param (
        [string]$TestPlan,
        [string]$ResultsFile,
        [string]$ReportDir,
        [string]$TestType
    )

    Write-Output "> Iniciando $TestType Test..."

    # Apagar relatórios antigos, se existirem
    if (Test-Path $ReportDir) {
        Remove-Item -Recurse -Force $ReportDir
        Write-Output "> Relatorio antigo de $TestType apagado."
    }

    # Apagar arquivo de resultados antigo, se existir
    if (Test-Path $ResultsFile) {
        Remove-Item $ResultsFile
        Write-Output "> Resultados antigos de $TestType apagados."
    }

    # Executar JMeter no modo CLI
    & $JMeterPath -n -t $TestPlan -l $ResultsFile -e -o $ReportDir

    Write-Output "> $TestType Test concluido! Relatorio disponivel em: $ReportDir"
}

# Executar ambos os testes
#Run-JMeterTest -TestPlan $LoadTestPlan -ResultsFile $LoadResultsFile -ReportDir $LoadReportDir -TestType "Load"

Start-Sleep -Seconds 20  # Pequena pausa para estabilização

Run-JMeterTest -TestPlan $StressTestPlan -ResultsFile $StressResultsFile -ReportDir $StressReportDir -TestType "Stress"
