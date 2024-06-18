using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextComparer;

namespace CompareSdlxliffLib
{
    internal class Program
    {
        static void Main(string[] args)
        {
			SdlxliffComparer comparer	= new SdlxliffComparer();
			//comparer.ReadSdlxliff(
			//	@"E:\DEV\Sdl-Community-master\Office\Examples\6014\6014_27.0_Source.docx_en-US_zh-CN.sdlxliff",
			//	false
			//	);
			comparer.Compare(
				@"E:\DEV\Sdl-Community-master\Office\Examples\6014\6014_27.0_Source.docx_en-US_zh-CN1.sdlxliff",
				@"E:\DEV\Sdl-Community-master\Office\Examples\6014\6014_27.0_Source.docx_en-US_zh-CN2.sdlxliff",
				true,true,ComparisonType.Words
				);
		}
    }
}
