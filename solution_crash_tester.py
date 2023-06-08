import subprocess
import sys
import time
from colorama import init as colorama_init
from colorama import Fore

colorama_init()

class Content:
  ONE = """
using Godot;

public class CrashTest 
{
    public void Test() 
    {
        GD.Print("Test");
    }
}
"""
  TWO = """
using Godot;

public class CrashTest 
{
    public void TestTwo() 
    {
        GD.Print("TestTwo");
    }
}
"""

DIVIDER = f"{Fore.LIGHTBLACK_EX}---------------------------------{Fore.RESET}"

begin_test_delay = 10;
delay = 0;
build_type = "dotnet"
if len(sys.argv) > 1 and sys.argv[1]:
   delay = int(sys.argv[1])
  
if len(sys.argv) > 2 and sys.argv[2]:
   build_type = sys.argv[2]

godot_process = subprocess.Popen(['godot', '--editor', '.'], stdout=subprocess.DEVNULL, stderr=subprocess.STDOUT)
required_success_count = 30;
success_count = 0

print(f"{Fore.LIGHTGREEN_EX}Solution Crash Tester(Delay: {delay} ms, Required Successes: {required_success_count}):{Fore.RESET}, Build CLI {build_type}")
print(f"{Fore.LIGHTGREEN_EX}  Press Ctrl+C to exit{Fore.RESET}")
print(DIVIDER)

time.sleep(begin_test_delay);
success = False

try:
  while True:
    content = ""
    try:
      with open("CrashTest.cs", "r+") as file:
        content = file.read()
        file.close()
    except OSError:
      print(f"{Fore.BLUE}  CrashTest.cs doesn't exist, creating new file{Fore.RESET}")
    with open("CrashTest.cs", "w+") as file:
      if content == Content.ONE:
         print(f"{Fore.BLUE}  Changing Content to TWO{Fore.RESET}")
         file.write(Content.TWO)
      else:
         print(f"{Fore.BLUE}  Changing Content to ONE{Fore.RESET}")
         file.write(Content.ONE)
      file.close()
    time.sleep(delay/1000)
    print(f"{Fore.LIGHTBLUE_EX}  Running Build:{Fore.RESET}")
    start = time.time()
    build_command = ["dotnet", "build"]
    if build_type == "msbuild":
      build_command = ["msbuild"]
    result = subprocess.run(build_command, stdout=subprocess.PIPE)
    end = time.time()
    if (result.returncode == 0):
      print(f"{Fore.BLUE}    Success (%.2f s){Fore.RESET}" % (end - start))
    else:
      print(f"{Fore.RED}    Build Failed:")
      print(f"{Fore.LIGHTBLACK_EX}{result.stdout.decode('utf-8')}{Fore.RESET}");
      break
    
    if godot_process.poll() != None:
      print(f"{Fore.RED}FAIL: Godot crashed. A marshalling bug exists in the project{Fore.RESET}");
      success = False
      break
    else:
      success_count += 1
      print(f"{Fore.BLUE}  Survived ({success_count}/{required_success_count}) rebuilds.{Fore.RESET}");
    
    if success_count >= required_success_count:
      print(f"{Fore.LIGHTGREEN_EX}SUCCESS: Godot survived the rebuilds.")
      success = True
      break
    print(DIVIDER)
except KeyboardInterrupt:
    pass

if godot_process.poll() == None:
  godot_process.kill()

print(f"{Fore.LIGHTGREEN_EX}Exited...{Fore.RESET}");

if success:
  sys.exit()
else:
  sys.exit(1)