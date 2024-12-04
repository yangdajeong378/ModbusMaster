using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ModbusMaster.Helpers
{
    public class CommandHelper
    {
        public static ICommand CancelCommand => new RelayCommand(() =>
        {
            // 모든 창에서 사용할 공통 동작
            Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive)?.Close();

            //Application.Current.Windows : WPF 애플리케이션에서 현재 열려 있는 모든 창을 나타내는 컬렉션

            //.ofType<Window>() : 모든 창을 포함한 컬렉션이므로, 특정 타입(Window)의 요소만 선택합니다.
            // => Window 타입의 창들만 필터링하여 리스트로 가져옵니다.

            //w => w.IsActive : LINQ(Language Integrated Query)를 사용하여 현재 활성화된 창을 찾습니다.
            //FirstOrDefault: 조건에 맞는 첫 번째 창을 찾고, 없으면 null을 반환합니다.
            // => 현재 활성화된 창이 반환됩니다. 활성화된 창이 없다면 null을 반환합니다.

            //조건부 연산자(?)를 사용하여, 활성화된 창이 있을 경우에만 Close() 메서드를 호출합니다.
            // close : 해당 창을 닫습니다.


            //*전체 코드 동작 과정*
            //현재 열려 있는 모든 창을 가져옵니다.
            //그중에서 활성화된 창(IsActive == true)을 찾습니다.
            //활성화된 창이 있다면, 해당 창을 닫습니다.
            //활성화된 창이 없다면 아무 작업도 하지 않습니다.
        });
    }
}
